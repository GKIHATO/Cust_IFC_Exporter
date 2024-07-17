using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Xbim.Ifc2x3.Kernel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Cust_IFC_Exporter
{
    public class SQLDBConnect
    {
        private SqlConnection connect;

        private string connectionString;

        static public int NumOfAverageMaterials = 0;

        public bool ClearAverageData()
        {
            using (SqlTransaction transaction = connect.BeginTransaction())
            {
                string[] tableNames = { "AverageDataTable", "AverageLocationTable", "AverageManufacturerTable", "AverageMaterialTable" };

                try
                {
                    foreach (string tableName in tableNames)
                    {
                        string deleteCommandText = "DELETE FROM [dbo].[{tableName}]";
                        // Create SqlCommand
                        using (SqlCommand command = new SqlCommand(deleteCommandText, connect, transaction))
                        {
                            // Execute the command
                            command.ExecuteNonQuery();
                        }
                    }
                    
                    // Commit the transaction
                    transaction.Commit();

                    MessageBox.Show("Total " + NumOfAverageMaterials.ToString() + " rows have been deleted");

                    NumOfAverageMaterials = 0;

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                    // Rollback the transaction if an error occurs
                    transaction.Rollback();

                    return false;
                }
            }
        }

        public bool ConnectDB(string connectionString)
        {
            try
            {
                connect = new SqlConnection(connectionString);

                if (connect.State == System.Data.ConnectionState.Closed)
                {
                    connect.Open();

                    NumOfAverageMaterials = GetAvergaeMaterialNum() + 1;

                    MessageBox.Show("Connect to database successfully!");
                }
                else
                {
                    connect.Close();

                    connect.Open();

                    NumOfAverageMaterials = GetAvergaeMaterialNum() + 1;

                    MessageBox.Show("Database is reconnected!");
                }

                return true;
            }
            catch
            {
                MessageBox.Show("Connect to database failed!");

                return false;
            }
        }

        private int GetAvergaeMaterialNum()
        {
           string queryString="SELECT COUNT(*) FROM [dbo].[AverageMaterialTable]";

            SqlCommand command = new SqlCommand(queryString, connect);

            int count = (int)command.ExecuteScalar();

            return count;
        }

        public void CloseDB()
        {
            if (connect.State == System.Data.ConnectionState.Open)
            {
                connect.Close();
            }
        }

        public SqlCommand Query(string query)
        {
            SqlCommand command = new SqlCommand(query, connect);

            return command;
        }

        public ObservableCollection<TreeNode<string>> GetMaterialTypes(List<string> manufacturerList, List<string> selectedRegions, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<TreeNode<string>> materialTypes = new ObservableCollection<TreeNode<string>>();
            string queryList = "";

            if (manufacturerList.Count > 0 && selectedRegions.Count > 0)
            {
                queryList += "SELECT DISTINCT [Material_Type] FROM [dbo].[MaterialTable] " +
                    "INNER JOIN [dbo].[FactoryList] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                    "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                    "WHERE [dbo].[MaterialTable].[Manufacturer_Name] IN ('" + manufacturerList[0] + "'";

                for (int i = 1; i < manufacturerList.Count; i++)
                {
                    queryList += ",'" + manufacturerList[i] + "'";
                }

                queryList += ") AND [dbo].[FactoryList].[Factory_CityName] IN ('" + selectedRegions[0] + "'";

                for (int i = 1; i < selectedRegions.Count; i++)
                {
                    queryList += ",'" + selectedRegions + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else if (manufacturerList.Count > 0 && selectedRegions.Count == 0)
            {
                queryList += "SELECT DISTINCT [Material_Type] FROM [dbo].[MaterialTable] " +
                    "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                    "WHERE [dbo].[MaterialTable].[Manufacturer_Name] IN ('" + manufacturerList[0] + "'";

                for (int i = 1; i < manufacturerList.Count; i++)
                {
                    queryList += ",'" + manufacturerList[i] + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else if (manufacturerList.Count == 0 && selectedRegions.Count > 0)
            {
                queryList += "SELECT DISTINCT [Material_Type] FROM [dbo].[MaterialTable] " +
                    "INNER JOIN [dbo].[FactoryList] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                    "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                    "WHERE [dbo].[FactoryList].[Factory_CityName] IN ('" + selectedRegions[0] + "'";

                for (int i = 1; i < selectedRegions.Count; i++)
                {
                    queryList += ",'" + selectedRegions + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else
            {
                queryList += "SELECT DISTINCT [Material_Type] FROM [dbo].[MaterialTable] " +
                    "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                    "WHERE [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND [dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";


            }

            SqlCommand manufacturerQuery = Query(queryList);

            manufacturerQuery.Parameters.AddWithValue("@startDate", startDate);

            manufacturerQuery.Parameters.AddWithValue("@endDate", endDate);

            using (SqlDataReader reader = manufacturerQuery.ExecuteReader())
            {
                while (reader.Read())
                {
                    TreeNode<string> materialType = new TreeNode<string>(reader.GetString(0));

                    materialTypes.Add(materialType);
                }
            }

            return organiseList(materialTypes);
        }

        public ObservableCollection<TreeNode<string>> GetRegionList(List<string> selectedMaterialTypes, List<string> manufacturerList, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<TreeNode<string>> regionList = new ObservableCollection<TreeNode<string>>();
            string queryList = "";

            if (selectedMaterialTypes.Count > 0 && manufacturerList.Count > 0)
            {
                queryList += "SELECT [Factory_CityName],[StateName],[CountryName] FROM [dbo].[FactoryList] " +
                    "INNER JOIN [dbo].[CountryCodeList] ON [dbo].[CountryCodeList].[CountryCode]=[dbo].[FactoryList].[Factory_CountryCode]" +
                    "INNER JOIN [dbo].[StateCodeList] ON [dbo].[StateCodeList].[StateCode]=[dbo].[FactoryList].[Factory_StateCode]" +
                    "INNER JOIN [dbo].[MaterialTable] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                    "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                    "WHERE [dbo].[MaterialTable].[Manufacturer_Name] IN ('" + manufacturerList[0] + "'";

                for (int i = 1; i < manufacturerList.Count; i++)
                {
                    queryList += ",'" + manufacturerList[i] + "'";
                }

                queryList += ") AND [dbo].[MaterialTable].[Material_Type] IN ('" + selectedMaterialTypes[0] + "'";

                for (int i = 1; i < selectedMaterialTypes.Count; i++)
                {
                    queryList += ",'" + selectedMaterialTypes + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else if (manufacturerList.Count > 0 && selectedMaterialTypes.Count == 0)
            {
                queryList += "SELECT [Factory_CityName],[StateName],[CountryName] FROM [dbo].[FactoryList] " +
                    "INNER JOIN [dbo].[CountryCodeList] ON [dbo].[CountryCodeList].[CountryCode]=[dbo].[FactoryList].[Factory_CountryCode]" +
                    "INNER JOIN [dbo].[StateCodeList] ON [dbo].[StateCodeList].[StateCode]=[dbo].[FactoryList].[Factory_StateCode]" +
                    "INNER JOIN [dbo].[MaterialTable] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                    "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                    "WHERE [dbo].[MaterialTable].[Manufacturer_Name] IN ('" + manufacturerList[0] + "'";

                for (int i = 1; i < manufacturerList.Count; i++)
                {
                    queryList += ",'" + manufacturerList[i] + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else if (manufacturerList.Count == 0 && selectedMaterialTypes.Count > 0)
            {
                queryList += "SELECT [Factory_CityName],[StateName],[CountryName] FROM [dbo].[FactoryList] " +
                    "INNER JOIN [dbo].[CountryCodeList] ON [dbo].[CountryCodeList].[CountryCode]=[dbo].[FactoryList].[Factory_CountryCode]" +
                    "INNER JOIN [dbo].[StateCodeList] ON [dbo].[StateCodeList].[StateCode]=[dbo].[FactoryList].[Factory_StateCode]" +
                    "INNER JOIN [dbo].[MaterialTable] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                    "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                    "WHERE [dbo].[MaterialTable].[Material_Type] IN ('" + selectedMaterialTypes[0] + "'";

                for (int i = 1; i < selectedMaterialTypes.Count; i++)
                {
                    queryList += ",'" + selectedMaterialTypes + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else
            {
                queryList += "SELECT [Factory_CityName],[StateName],[CountryName] FROM [dbo].[FactoryList] " +
                    "INNER JOIN [dbo].[CountryCodeList] ON [dbo].[CountryCodeList].[CountryCode]=[dbo].[FactoryList].[Factory_CountryCode]" +
                    "INNER JOIN [dbo].[StateCodeList] ON [dbo].[StateCodeList].[StateCode]=[dbo].[FactoryList].[Factory_StateCode]" +
                    "INNER JOIN [dbo].[MaterialTable] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                    "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                    "WHERE [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND [dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }

            SqlCommand manufacturerQuery = Query(queryList);

            manufacturerQuery.Parameters.AddWithValue("@startDate", startDate);

            manufacturerQuery.Parameters.AddWithValue("@endDate", endDate);

            using (SqlDataReader reader = manufacturerQuery.ExecuteReader())
            {
                while (reader.Read())
                {
                    string cityName = reader.GetString(0);
                    string stateName = reader.GetString(1);
                    string countryName = reader.GetString(2);

/*                    if (stateName == "N/A")
                    {
                        if (regionList.Where(x => x.Data == countryName).Count() == 0)
                        {
                            TreeNode<string> regionCountry = new TreeNode<string>(countryName);

                            regionList.Add(regionCountry);

                            TreeNode<string> regionCity = new TreeNode<string>(cityName);

                            regionCountry.Children.Add(regionCity);

                            regionCity.ParentNode = regionCountry;

                        }
                        else
                        {
                            TreeNode<string> regionCountry = regionList.Where(x => x.Data == countryName).First();

                            if (regionCountry.Children.Where(x => x.Data == cityName).Count() == 0)
                            {
                                TreeNode<string> regionCity = new TreeNode<string>(cityName);

                                regionCountry.Children.Add(regionCity);

                                regionCity.ParentNode = regionList.Where(x => x.Data == countryName).First();
                            }
                        }
                    }
                    else
                    {*/
                        if (regionList.Where(x => x.Data == countryName).Count() == 0)
                        {
                            TreeNode<string> regionCountry = new TreeNode<string>(countryName);

                            TreeNode<string> regionState = new TreeNode<string>(stateName);

                            TreeNode<string> regionCity = new TreeNode<string>(cityName);

                            regionCountry.Children.Add(regionState);

                            regionState.ParentNode = regionCountry;

                            regionState.Children.Add(regionCity);

                            regionCity.ParentNode = regionState;

                            regionList.Add(regionCountry);
                        }
                        else
                        {
                            TreeNode<string> regionCountry = regionList.Where(x => x.Data == countryName).First();

                            if (regionCountry.Children.Where(x => x.Data == stateName).Count() == 0)
                            {
                                TreeNode<string> regionState = new TreeNode<string>(stateName);

                                regionCountry.Children.Add(regionState);

                                regionState.ParentNode = regionCountry;

                                TreeNode<string> regionCity = new TreeNode<string>(cityName);

                                regionState.Children.Add(regionCity);

                                regionCity.ParentNode = regionState;

                            }
                            else
                            {
                                TreeNode<string> regionState = regionCountry.Children.Where(x => x.Data == stateName).First();

                                if (regionState.Children.Where(x => x.Data == cityName).Count() == 0)
                                {
                                    TreeNode<string> regionCity = new TreeNode<string>(cityName);

                                    regionState.Children.Add(regionCity);

                                    regionCity.ParentNode = regionState;
                                }
                            }
                        }
                    //}
                }
            }

            return organiseList(regionList);
        }

        private List<TreeNode<string>> organiseList(List<TreeNode<string>> List)
        {
            foreach (var child in List)
            {
                if (child.Children.Count > 0)
                {
                    child.Children = organiseList(child.Children);
                }
            }

            return List.OrderBy(x => x.Data).ToList();
        }

        private ObservableCollection<TreeNode<string>> organiseList(ObservableCollection<TreeNode<string>> List)
        {
            ObservableCollection<TreeNode<string>> organisedList = new ObservableCollection<TreeNode<string>>(List.OrderBy(x => x.Data));

            foreach (TreeNode<string> node in organisedList)
            {
                if (node.Children.Count > 0)
                {
                    node.Children = organiseList(node.Children);
                }
            }

            return organisedList;
        }

        public ObservableCollection<TreeNode<string>> GetManufacturerList(List<string> selectedMaterialTypes, List<string> selectedRegions, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<TreeNode<string>> manufacturerList = new ObservableCollection<TreeNode<string>>();
            string queryList = "";

            if (selectedMaterialTypes.Count > 0 && selectedRegions.Count > 0)
            {
                queryList += "SELECT DISTINCT [dbo].[MaterialTable].[Manufacturer_Name] FROM [dbo].[MaterialTable] " +
                    "INNER JOIN [dbo].[FactoryList] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                    "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                    "WHERE [dbo].[MaterialTable].[Material_Type] IN ('" + selectedMaterialTypes[0] + "'";

                for (int i = 1; i < selectedMaterialTypes.Count; i++)
                {
                    queryList += ",'" + selectedMaterialTypes[i] + "'";
                }

                queryList += ") AND [dbo].[FactoryList].[Factory_CityName] IN ('" + selectedRegions[0] + "'";

                for (int i = 1; i < selectedRegions.Count; i++)
                {
                    queryList += ",'" + selectedRegions + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else if (selectedMaterialTypes.Count > 0 && selectedRegions.Count == 0)
            {
                queryList += "SELECT DISTINCT [dbo].[MaterialTable].[Manufacturer_Name] FROM [dbo].[MaterialTable] " +
                    "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                    "WHERE [dbo].[MaterialTable].[Material_Type] IN ('" + selectedMaterialTypes[0] + "'";

                for (int i = 1; i < selectedMaterialTypes.Count; i++)
                {
                    queryList += ",'" + selectedMaterialTypes[i] + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else if (selectedMaterialTypes.Count == 0 && selectedRegions.Count > 0)
            {
                queryList += "SELECT DISTINCT [dbo].[MaterialTable].[Manufacturer_Name] FROM [dbo].[MaterialTable] " +
                    "INNER JOIN [dbo].[FactoryList] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                    "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                    "WHERE [dbo].[FactoryList].[Factory_CityName] IN ('" + selectedRegions[0] + "'";

                for (int i = 1; i < selectedRegions.Count; i++)
                {
                    queryList += ",'" + selectedRegions + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else
            {
                queryList += "SELECT DISTINCT [dbo].[MaterialTable].[Manufacturer_Name] FROM [dbo].[MaterialTable] " +
                    "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                    "WHERE [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND [dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }

            SqlCommand manufacturerQuery = Query(queryList);

            manufacturerQuery.Parameters.AddWithValue("@startDate", startDate);

            manufacturerQuery.Parameters.AddWithValue("@endDate", endDate);

            using (SqlDataReader reader = manufacturerQuery.ExecuteReader())
            {
                while (reader.Read())
                {
                    TreeNode<string> manufacturer = new TreeNode<string>(reader.GetString(0));

                    manufacturerList.Add(manufacturer);
                }
            }

            return organiseList(manufacturerList);
        }

        public void LoadData(Dictionary<string, List<string>> rawData, RowItem rowData, bool generateAverageData)
        {
            if (rowData.selectedTypes.Count > 0)
            {
                foreach (string selectedMaterialType in rowData.selectedTypes)
                {
                    List<string> rawDatabyType = LoadDataByType(selectedMaterialType, rowData, generateAverageData);

                    if (!rawData.ContainsKey(selectedMaterialType))
                    {
                        rawData.Add(selectedMaterialType, rawDatabyType);
                    }
                    else
                    {
                        rawData[selectedMaterialType].AddRange(rawDatabyType);
                    }
                }
            }
            else
            {
                MessageBox.Show("Filter with no material type selected will be skipped!");
            }
           
        }

        private List<string> LoadDataByType(string selectedMaterialType, RowItem rowData, bool generateAverageData)
        {
            string[] typeList = { "Aggregates", "Cement", "Fibre", "Lime", "Masonry", "ReadyMixConcrete", "Metal","Timber", "Door", "Window", "Furniture", "Tank", "PrecastConcrete", "Reinforcement" };

            switch (selectedMaterialType)
            {
                case "ReadyMixConcrete":
                    return LoadConcreteData(rowData, generateAverageData);
                case "Timber":
                    return LoadTimberData(rowData, generateAverageData);
/*                case "Aggregates":
                    return LoadAggregateData(selectedManufacturers, selectedRegions, startDate, endDate);
                case "Cement":
                    return LoadCementeData(selectedManufacturers, selectedRegions, startDate, endDate);
                case "Fibre":
                    return LoadFibreData(selectedManufacturers, selectedRegions, startDate, endDate);
                case "Lime":
                    return LoadLimeData(selectedManufacturers, selectedRegions, startDate, endDate);*/
  /*              case "Masonry":
                    return LoadMasonryData(rowData, generateAverageData);
                case "Metal":
                    return LoadMetalData(rowData, generateAverageData);
*//*                case "Door":
                    return LoadDoorData(selectedManufacturers, selectedRegions, startDate, endDate);
                case "Window":
                    return LoadWindowData(selectedManufacturers, selectedRegions, startDate, endDate);
                case "Furniture":
                    return LoadFurnitureData(selectedManufacturers, selectedRegions, startDate, endDate);
                case "Tank":
                    return LoadTankData(selectedManufacturers, selectedRegions, startDate, endDate);*//*
                case "PrecastConcrete":
                    return LoadPrecastConcreteData(rowData, generateAverageData);
                case "Reinforcement":
                    return LoadReinforcementData(rowData, generateAverageData);*/
                default:
                    return LoadGeneralTypeData(selectedMaterialType,rowData, generateAverageData);
            }
        }

        private List<string> LoadReinforcementData(RowItem rowItem, bool generateAverageData)
        {
            throw new NotImplementedException();
        }

        private List<string> LoadPrecastConcreteData(RowItem rowItem, bool generateAverageData)
        {
            throw new NotImplementedException();
        }

        private List<string> LoadMetalData(RowItem rowItem, bool generateAverageData)
        {
            throw new NotImplementedException();
        }

        private List<string> LoadMasonryData(RowItem rowItem, bool generateAverageData)
        {
            throw new NotImplementedException();
        }

        /*private List<string> LoadLimeData(List<string> selectedManufacturers, List<string> selectedRegions, string startDate, string endDate)
        {
            throw new NotImplementedException();
        }

        private List<string> LoadFibreData(List<string> selectedManufacturers, List<string> selectedRegions, string startDate, string endDate)
        {
            throw new NotImplementedException();
        }

        private List<string> LoadCementeData(List<string> selectedManufacturers, List<string> selectedRegions, string startDate, string endDate)
        {
            throw new NotImplementedException();
        }

        private List<string> LoadAggregateData(List<string> selectedManufacturers, List<string> selectedRegions, string startDate, string endDate)
        {
            throw new NotImplementedException();
        }*/

        private List<string> LoadConcreteData(RowItem rowData, bool generateAverageData)
        {
            string queryList = null;

            if (rowData.selectedManufacturers.Count > 0 && rowData.selectedCities.Count > 0)
            {

                queryList = "SELECT [dbo].[MaterialTable].[Material_ID],[Material_Type],[dbo].[MaterialTable].[Material_Name],[dbo].[MaterialTable].[Manufacturer_ID],[dbo].[MaterialTable].[Manufacturer_Name],[Model_Code],[Concrete_Strength],[Concrete_Density(kg/m3)] FROM [dbo].[MaterialTable] " +
                "INNER JOIN [dbo].[FactoryList] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                "INNER JOIN [dbo].[ReadyMixConcreteTable] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[ReadyMixConcreteTable].[Material_ID]" +
                "WHERE [dbo].[MaterialTable].[Material_Type] = 'ReadyMixConcrete'" +
                "AND [dbo].[MaterialTable].[Manufacturer_Name] IN ('" + rowData.selectedManufacturers[0] + "'";

                for (int i = 1; i < rowData.selectedManufacturers.Count; i++)
                {
                    queryList += ",'" + rowData.selectedManufacturers[i] + "'";
                }

                queryList += ") AND [dbo].[FactoryList].[Factory_CityName] IN ('" + rowData.selectedCities[0] + "'";

                for (int i = 1; i < rowData.selectedCities.Count; i++)
                {
                    queryList += ",'" + rowData.selectedCities[i] + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else if (rowData.selectedManufacturers.Count > 0 && rowData.selectedCities.Count == 0)
            {
                queryList = "SELECT [dbo].[MaterialTable].[Material_ID],[Material_Type],[dbo].[MaterialTable].[Material_Name],[dbo].[MaterialTable].[Manufacturer_ID],[dbo].[MaterialTable].[Manufacturer_Name],[Model_Code],[Concrete_Strength],[Concrete_Density(kg/m3)] FROM [dbo].[MaterialTable] " +
                "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                "INNER JOIN [dbo].[ReadyMixConcreteTable] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[ReadyMixConcreteTable].[Material_ID]"+
                "WHERE [dbo].[MaterialTable].[Material_Type] = 'ReadyMixConcrete'" +
                "AND [dbo].[MaterialTable].[Manufacturer_Name] IN ('" + rowData.selectedManufacturers[0] + "'";

                for (int i = 1; i < rowData.selectedManufacturers.Count; i++)
                {
                    queryList += ",'" + rowData.selectedManufacturers[i] + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else if (rowData.selectedManufacturers.Count == 0 && rowData.selectedCities.Count > 0)
            {
                queryList = "SELECT [dbo].[MaterialTable].[Material_ID],[Material_Type],[dbo].[MaterialTable].[Material_Name],[dbo].[MaterialTable].[Manufacturer_ID],[dbo].[MaterialTable].[Manufacturer_Name],[Model_Code],[Concrete_Strength],[Concrete_Density(kg/m3)] FROM [dbo].[MaterialTable] " +
                "INNER JOIN [dbo].[FactoryList] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                "INNER JOIN [dbo].[ReadyMixConcreteTable] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[ReadyMixConcreteTable].[Material_ID]"+
                "WHERE [dbo].[MaterialTable].[Material_Type] = 'ReadyMixConcrete'" +
                "AND [dbo].[FactoryList].[Factory_CityName] IN ('" + rowData.selectedCities[0] + "'";

                for (int i = 1; i < rowData.selectedCities.Count; i++)
                {
                    queryList += ",'" + rowData.selectedCities[i] + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else
            {
                queryList = "SELECT [dbo].[MaterialTable].[Material_ID],[Material_Type],[dbo].[MaterialTable].[Material_Name],[dbo].[MaterialTable].[Manufacturer_ID],[dbo].[MaterialTable].[Manufacturer_Name],[Model_Code],[Concrete_Strength],[Concrete_Density(kg/m3)] FROM [dbo].[MaterialTable] " +
                "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                "INNER JOIN [dbo].[ReadyMixConcreteTable] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[ReadyMixConcreteTable].[Material_ID]"+
                "WHERE [dbo].[MaterialTable].[Material_Type] = 'ReadyMixConcrete'" +
                "AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND [dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }

            if (queryList != null)
            {
                SqlCommand concreteDataQuery = Query(queryList);

                string[] dates = rowData.DateRange.Split(new[] { " - " }, StringSplitOptions.None);

                concreteDataQuery.Parameters.AddWithValue("@startDate", dates[0]);

                concreteDataQuery.Parameters.AddWithValue("@endDate", dates[1]);

                List<string> rawData = new List<string>();

                int count = 0;

                HashSet<(int,string)> manufacturers = new HashSet<(int,string)>();

                List<Guid> ids = new List<Guid>();

                List<double> strengths = new List<double>();

                List<decimal> densities = new List<decimal>();

                using (SqlDataReader reader = concreteDataQuery.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string record = "";

                        record += reader.GetGuid(0).ToString() + "\t";

                        for (int i = 1; i < 6; i++)
                        {
                            if (i == 3)
                            {
                                record += reader.GetInt32(3) + "\t";
                            }
                            else
                            {
                                record += reader.GetString(i) + "\t";
                            }                        
                        }
                        string valueWithoutUnit = null;

                        if (!string.IsNullOrWhiteSpace(reader.GetString(6)))
                        {
                            valueWithoutUnit = Regex.Replace(reader.GetString(6), @"MPa\s*", "");
                        }
                        else
                        {
                            valueWithoutUnit = "0";
                        }

                        record += valueWithoutUnit + "\t";

                        record += reader.GetDecimal(7).ToString();

                        rawData.Add(record);

                        if (generateAverageData)
                        {
                            manufacturers.Add((reader.GetInt32(3),reader.GetString(4)));

                            count++;

                            ids.Add(reader.GetGuid(0));

                            if (double.TryParse(valueWithoutUnit, out double result))
                            {
                                if (result != 0)
                                {
                                    strengths.Add(result);
                                }
                            }

                            densities.Add(reader.GetDecimal(7));
                        }
                    }
                }


                if (generateAverageData)
                {
                    manufacturers = manufacturers.Distinct().ToHashSet();

                    string averageRecord=InsertData(rowData, "ReadyMixConcrete", manufacturers, ids);

                    if(averageRecord!=null)
                    {
                        double averageStrength = strengths.Average();

                        decimal averageDensity = densities.Average();

                        averageRecord += "\t" + averageStrength.ToString();

                        averageRecord += "\t" + averageDensity.ToString();

                        rawData.Add(averageRecord);
                    }
                }

                return rawData;
            }
            else
            {
                MessageBox.Show("Something went wrong!");

                return null;
            }
        }


        private string InsertData(RowItem rowItem, string selectedType, HashSet<(int,string)> manufacturers, List<Guid> ids)
        {

            Guid averageMaterialID = Guid.NewGuid();

            NumOfAverageMaterials++;

            DateTime timeStamp = DateTime.Now;

            string averageDataRecord = "No." + NumOfAverageMaterials.ToString() + "_" + selectedType + "_" + timeStamp;

            string record = "";

            string[] dates = rowItem.DateRange.Split(new[] { " - " }, StringSplitOptions.None);

            record += averageMaterialID.ToString() + "\t";

            record += selectedType + "\t";

            record += averageDataRecord + "\t";

            if(manufacturers.Count > 1)
            {
                record += "N/A" + "\t";

                record += "Multiple" + "\t";
            }
            else if(manufacturers.Count == 1)
            {
                (int, string) pair = manufacturers.FirstOrDefault();

                record += pair.Item1.ToString() + "\t";

                record += pair.Item2 + "\t";
            }
            else
            {
                record += "N/A" + "\t";

                record += "N/A" + "\t";
            }

            string comment = selectedType + " average data generated from: \nRegions: " + rowItem.Region + "\nManufacturers: " + rowItem.Manufacturer + "\nTime Range: " + dates[0] + " to " + dates[1] + "\nRecord Number: " + ids.Count.ToString() + "\nTime Stamp: " + timeStamp.ToString("yyyy-MM-dd HH:mm:ss");

            record += comment;

            var transaction = connect.BeginTransaction();

            try
            {

                SqlCommand enableIdentityInsertCommand = new SqlCommand("SET IDENTITY_INSERT [dbo].[AverageMaterialTable] ON", connect,transaction);

                enableIdentityInsertCommand.ExecuteNonQuery();

                using (var insertCommand = new SqlCommand("INSERT INTO [dbo].[AverageMaterialTable] ([No.],[Material_ID],[Material_Name], [Material_Type],[NumOfRecords],[NumOfRegions],[NumOfManufacturers],[StartDate],[EndDate],[GenerateTime]) VALUES (@No,@Material_ID,@Material_Name, @Material_Type,@NumOfRecords,@NumOfRegions,@NumofManufacturers,@StartDate,@EndDate,@GenerateTime)", connect, transaction))
                {
                    // Set parameter values
                    insertCommand.Parameters.AddWithValue("@No", NumOfAverageMaterials);

                    insertCommand.Parameters.AddWithValue("@Material_ID", averageMaterialID);

                    insertCommand.Parameters.AddWithValue("@Material_Name", averageDataRecord);

                    insertCommand.Parameters.AddWithValue("@Material_Type", selectedType);

                    insertCommand.Parameters.AddWithValue("@NumOfRecords", ids.Count);

                    insertCommand.Parameters.AddWithValue("@NumOfRegions", rowItem.selectedCities.Count);//0 represents no limits

                    insertCommand.Parameters.AddWithValue("@NumofManufacturers", manufacturers.Count);//0 represents 0

                    insertCommand.Parameters.AddWithValue("@StartDate", dates[0]);

                    insertCommand.Parameters.AddWithValue("@EndDate", dates[1]);

                    insertCommand.Parameters.AddWithValue("@GenerateTime", timeStamp);

                    // Execute INSERT command
                    insertCommand.ExecuteNonQuery();
                }

                foreach (var manufacturer in manufacturers)
                {
                    using (var insertCommand = new SqlCommand("INSERT INTO [dbo].[AverageManufacturerTable] ([Manufacturer_ID], [AverageMaterial_ID]) VALUES (@Manufacturer_ID, @AverageMaterial_ID)", connect, transaction))
                    {
                        // Set parameter values
                        insertCommand.Parameters.AddWithValue("@AverageMaterial_ID", averageMaterialID);

                        insertCommand.Parameters.AddWithValue("@Manufacturer_ID", manufacturer.Item1);

                        // Execute INSERT command
                        insertCommand.ExecuteNonQuery();
                    }
                }

                foreach (var region in rowItem.selectedCities)
                {
                    using (var insertCommand = new SqlCommand("INSERT INTO [dbo].[AverageLocationTable] ([City_Name], [AverageMaterial_ID]) VALUES (@City_Name, @AverageMaterial_ID)", connect, transaction))
                    {
                        // Set parameter values
                        insertCommand.Parameters.AddWithValue("@AverageMaterial_ID", averageMaterialID);

                        insertCommand.Parameters.AddWithValue("@City_Name", region);

                        // Execute INSERT command
                        insertCommand.ExecuteNonQuery();
                    }
                }

                foreach (var id in ids)
                {
                    using (var insertCommand = new SqlCommand("INSERT INTO [dbo].[AverageDataTable] ([Material_ID], [AverageMaterial_ID]) VALUES (@Material_ID, @AverageMaterial_ID)", connect, transaction))
                    {
                        // Set parameter values

                        insertCommand.Parameters.AddWithValue("@Material_ID", id);

                        insertCommand.Parameters.AddWithValue("@AverageMaterial_ID", averageMaterialID);

                        // Execute INSERT command
                        insertCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();

                return record;
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return null;
            }
        }

        private List<string> LoadTimberData(RowItem rowItem, bool generateAverageData)
        {
            string queryList = null;
            if (rowItem.selectedManufacturers.Count > 0 && rowItem.selectedCities.Count > 0)
            {

                queryList = "SELECT [dbo].[MaterialTable].[Material_ID],[Material_Type],[dbo].[MaterialTable].[Material_Name],[dbo].[MaterialTable].[Manufacturer_ID],[dbo].[MaterialTable].[Manufacturer_Name],[Model_Code],[Timber_Strength],[Timber_Density(kg/m3)] FROM [dbo].[MaterialTable] " +
                "INNER JOIN [dbo].[FactoryList] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                "INNER JOIN [dbo].[TimberTable] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[TimberTable].[Material_ID]" +
                "WHERE [dbo].[MaterialTable].[Material_Type] = 'Timber'" +
                "AND [dbo].[MaterialTable].[Manufacturer_Name] IN ('" + rowItem.selectedManufacturers[0] + "'";

                for (int i = 1; i < rowItem.selectedManufacturers.Count; i++)
                {
                    queryList += ",'" + rowItem.selectedManufacturers[i] + "'";
                }

                queryList += ") AND [dbo].[FactoryList].[Factory_CityName] IN ('" + rowItem.selectedCities[0] + "'";

                for (int i = 1; i < rowItem.selectedCities.Count; i++)
                {
                    queryList += ",'" + rowItem.selectedCities[i] + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else if (rowItem.selectedManufacturers.Count > 0 && rowItem.selectedCities.Count == 0)
            {
                queryList = "SELECT [dbo].[MaterialTable].[Material_ID],[Material_Type],[dbo].[MaterialTable].[Material_Name],[dbo].[MaterialTable].[Manufacturer_ID],[dbo].[MaterialTable].[Manufacturer_Name],[Model_Code],[Timber_Strength],[Timber_Density(kg/m3)] FROM [dbo].[MaterialTable] " +
                "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                "INNER JOIN [dbo].[TimberTable] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[TimberTable].[Material_ID]" +
                "WHERE [dbo].[MaterialTable].[Material_Type] = 'Timber'" +
                "AND [dbo].[MaterialTable].[Manufacturer_Name] IN ('" + rowItem.selectedManufacturers[0] + "'";

                for (int i = 1; i < rowItem.selectedManufacturers.Count; i++)
                {
                    queryList += ",'" + rowItem.selectedManufacturers[i] + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else if (rowItem.selectedManufacturers.Count == 0 && rowItem.selectedCities.Count > 0)
            {
                queryList = "SELECT [dbo].[MaterialTable].[Material_ID],[Material_Type],[dbo].[MaterialTable].[Material_Name],[dbo].[MaterialTable].[Manufacturer_ID],[dbo].[MaterialTable].[Manufacturer_Name],[Model_Code],[Timber_Strength],[Timber_Density(kg/m3)] FROM [dbo].[MaterialTable] " +
                "INNER JOIN [dbo].[FactoryList] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                "INNER JOIN [dbo].[TimberTable] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[TimberTable].[Material_ID]" +
                "WHERE [dbo].[MaterialTable].[Material_Type] = 'Timber'" +
                "AND [dbo].[FactoryList].[Factory_CityName] IN ('" + rowItem.selectedCities[0] + "'";

                for (int i = 1; i < rowItem.selectedCities.Count; i++)
                {
                    queryList += ",'" + rowItem.selectedCities[i] + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else
            {
                queryList = "SELECT [dbo].[MaterialTable].[Material_ID],[Material_Type],[dbo].[MaterialTable].[Material_Name],[dbo].[MaterialTable].[Manufacturer_ID],[dbo].[MaterialTable].[Manufacturer_Name],[Model_Code],[Timber_Strength],[Timber_Density(kg/m3)] FROM [dbo].[MaterialTable] " +
                "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                "INNER JOIN [dbo].[TimberTable] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[TimberTable].[Material_ID]" +
                "WHERE [dbo].[MaterialTable].[Material_Type] = 'Timber'" +
                "AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND [dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }

            if (queryList != null)
            {
                SqlCommand timberDataQuery = Query(queryList);

                string[] dates = rowItem.DateRange.Split(new[] { " - " }, StringSplitOptions.None);

                timberDataQuery.Parameters.AddWithValue("@startDate", dates[0]);

                timberDataQuery.Parameters.AddWithValue("@endDate", dates[1]);

                List<string> rawData = new List<string>();

                int count = 0;

                HashSet<(int,string)> manufacturers = new HashSet<(int,string)>();

                List<Guid> ids = new List<Guid>();

                List<double> strengths = new List<double>();

                List<decimal> densities = new List<decimal>();

                using (SqlDataReader reader = timberDataQuery.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string record = "";

                        record += reader.GetGuid(0).ToString() + "\t";

                        for (int i = 1; i < 6; i++)
                        {
                            if (i == 3)
                            {
                                record += reader.GetInt32(3) + "\t";
                            }
                            else
                            {
                                record += reader.GetString(i) + "\t";
                            }
                        }

                        string valueWithoutUnit = reader.GetString(6);

                        if (string.IsNullOrWhiteSpace(valueWithoutUnit))
                        {
                            valueWithoutUnit = "0";
                        }

                        record += valueWithoutUnit + "\t";

                        record += reader.GetDecimal(7).ToString();

                        rawData.Add(record);

                        if (generateAverageData)
                        {
                            manufacturers.Add((reader.GetInt32(3), reader.GetString(4)));

                            count++;

                            ids.Add(reader.GetGuid(0));

                            if (double.TryParse(valueWithoutUnit, out double result))
                            {
                                if (result != 0)
                                {
                                    strengths.Add(result);
                                }
                            }

                            densities.Add(reader.GetDecimal(7));

                        }
                    }
                }

                if (generateAverageData)
                {
                    manufacturers = manufacturers.Distinct().ToHashSet();

                    string averageRecord = InsertData(rowItem, "Timber", manufacturers, ids);

                    if (averageRecord != null)
                    {
                        double averageStrength = 0;

                        if (strengths.Count > 0)
                        {
                            averageStrength = strengths.Average();
                        }

                        decimal averageDensity = 0;

                        if (densities.Count > 0)
                        {
                            averageDensity = densities.Average();
                        }

                        averageRecord += "\t" + averageStrength.ToString();

                        averageRecord += "\t" + averageDensity.ToString();

                        rawData.Add(averageRecord);
                    }
                }
                   

                return rawData;
            }
            else
            {
                MessageBox.Show("Something went wrong!");

                return null;
            }
        }

        private List<string> LoadGeneralTypeData(string materialType, RowItem rowItem, bool generateAverageData)
        {
            string queryList = null;
            if (rowItem.selectedManufacturers.Count > 0 && rowItem.selectedCities.Count > 0)
            {

                queryList = "SELECT [dbo].[MaterialTable].[Material_ID],[Material_Type],[dbo].[MaterialTable].[Material_Name],[dbo].[MaterialTable].[Manufacturer_ID],[dbo].[MaterialTable].[Manufacturer_Name],[Model_Code] FROM [dbo].[MaterialTable] " +
                "INNER JOIN [dbo].[FactoryList] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                "WHERE [dbo].[MaterialTable].[Material_Type] = '" + materialType +
                "' AND [dbo].[MaterialTable].[Manufacturer_Name] IN ('" + rowItem.selectedManufacturers[0] + "'";

                for (int i = 1; i < rowItem.selectedManufacturers.Count; i++)
                {
                    queryList += ",'" + rowItem.selectedManufacturers[i] + "'";
                }

                queryList += ") AND [dbo].[FactoryList].[Factory_CityName] IN ('" + rowItem.selectedCities[0] + "'";

                for (int i = 1; i < rowItem.selectedCities.Count; i++)
                {
                    queryList += ",'" + rowItem.selectedCities[i] + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else if (rowItem.selectedManufacturers.Count > 0 && rowItem.selectedCities.Count == 0)
            {
                queryList = "SELECT [dbo].[MaterialTable].[Material_ID],[Material_Type],[dbo].[MaterialTable].[Material_Name],[dbo].[MaterialTable].[Manufacturer_ID],[dbo].[MaterialTable].[Manufacturer_Name],[Model_Code] FROM [dbo].[MaterialTable] " +
                "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                "WHERE [dbo].[MaterialTable].[Material_Type] = '" + materialType +
                "' AND [dbo].[MaterialTable].[Manufacturer_Name] IN ('" + rowItem.selectedManufacturers[0] + "'";

                for (int i = 1; i < rowItem.selectedManufacturers.Count; i++)
                {
                    queryList += ",'" + rowItem.selectedManufacturers[i] + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else if (rowItem.selectedManufacturers.Count == 0 && rowItem.selectedCities.Count > 0)
            {
                queryList = "SELECT [dbo].[MaterialTable].[Material_ID],[Material_Type],[dbo].[MaterialTable].[Material_Name],[dbo].[MaterialTable].[Manufacturer_ID],[dbo].[MaterialTable].[Manufacturer_Name],[Model_Code] FROM [dbo].[MaterialTable] " +
                "INNER JOIN [dbo].[FactoryList] ON [dbo].[MaterialTable].[Factory_ID]=[dbo].[FactoryList].[Factory_ID]" +
                "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                "WHERE [dbo].[MaterialTable].[Material_Type] = '" + materialType +
                "' AND [dbo].[FactoryList].[Factory_CityName] IN ('" + rowItem.selectedCities[0] + "'";

                for (int i = 1; i < rowItem.selectedCities.Count; i++)
                {
                    queryList += ",'" + rowItem.selectedCities[i] + "'";
                }

                queryList += ") AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND[dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }
            else
            {
                queryList = "SELECT [dbo].[MaterialTable].[Material_ID],[Material_Type],[dbo].[MaterialTable].[Material_Name],[dbo].[MaterialTable].[Manufacturer_ID],[dbo].[MaterialTable].[Manufacturer_Name],[Model_Code] FROM [dbo].[MaterialTable] " +
                "INNER JOIN [dbo].[LCA_GeneralInfo] ON [dbo].[MaterialTable].[Material_ID] = [dbo].[LCA_GeneralInfo].[Material_ID]" +
                "WHERE [dbo].[MaterialTable].[Material_Type] = '" + materialType +
                "' AND [dbo].[LCA_GeneralInfo].[ValidFrom] < @startDate AND [dbo].[LCA_GeneralInfo].[ValidUntil] > @endDate";
            }

            if (queryList != null)
            {
                SqlCommand rawDataQuery = Query(queryList);

                string[] dates = rowItem.DateRange.Split(new[] { " - " }, StringSplitOptions.None);

                rawDataQuery.Parameters.AddWithValue("@startDate", dates[0]);

                rawDataQuery.Parameters.AddWithValue("@endDate", dates[1]);

                List<string> rawData= new List<string>();

                int count = 0;

                HashSet<(int,string)> manufacturers = new HashSet<(int, string)>();

                List<Guid> ids = new List<Guid>();

                using (SqlDataReader reader = rawDataQuery.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string record = "";

                        record += reader.GetGuid(0).ToString() + "\t";

                        for (int i = 1; i < 6; i++)
                        {
                            if (i == 3)
                            {
                                record += reader.GetInt32(3) + "\t";
                            }
                            else
                            {
                                record += reader.GetString(i) + "\t";
                            }
                        }

                        rawData.Add(record);

                        if (generateAverageData)
                        {
                            manufacturers.Add((reader.GetInt32(3), reader.GetString(4)));

                            count++;

                            ids.Add(reader.GetGuid(0));
                        }
                    }
                }

                    if (generateAverageData)
                    {
                        manufacturers = manufacturers.Distinct().ToHashSet();

                        string averageRecord = InsertData(rowItem, materialType, manufacturers, ids);

                        if (averageRecord != null)
                        {
                            rawData.Add(averageRecord);
                        }
                    }
                
                return rawData;
            }
            else
            {
                MessageBox.Show("Something went wrong!");

                return null;
            }
        }
    }
}
