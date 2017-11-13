using EndToEnd.Models;
using EndToEnd.Util;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace EndToEnd.Managers
{
    public static class ActiveDirectoryManager
    {
        public static List<ActiveDirectoryModel> GetDetailsFromAD(string SearchText)
        {
            List<ActiveDirectoryModel> listAD = null;
            ActiveDirectoryModel objAD = null;
            try
            {
                listAD = new List<ActiveDirectoryModel>();

                if (!string.IsNullOrEmpty(ConstHelpers.Str_ADDomainName) && !string.IsNullOrEmpty(ConstHelpers.Str_ADLDAPPath))
                {
                    DirectoryEntry entry = new DirectoryEntry(ConstHelpers.Str_ADLDAPPath, ConstHelpers.Str_ADUserName, ConstHelpers.Str_ADPassword);
                    object obj = entry.NativeObject;
                    DirectorySearcher search = new DirectorySearcher(entry);
                    search.Filter = ConstHelpers.Str_ADFilter;
                    search.PropertiesToLoad.Add(ConstHelpers.Str_ADKey_AcctName);
                    search.PropertiesToLoad.Add(ConstHelpers.Str_ADKey_Mail);
                    foreach (SearchResult resEnt in search.FindAll())
                    {
                        objAD = new ActiveDirectoryModel();
                        entry = resEnt.GetDirectoryEntry();
                        if (entry.Properties[ConstHelpers.Str_ADKey_AcctName].Value != null && entry.Properties[ConstHelpers.Str_ADKey_UsrAcctCtrl].Value != null)
                        {
                            objAD.UserName = Convert.ToString(entry.Properties[ConstHelpers.Str_ADKey_AcctName].Value);
                            objAD.EmailID = Convert.ToString(entry.Properties[ConstHelpers.Str_ADKey_Mail].Value);
                            objAD.FirstName = Convert.ToString(entry.Properties["displayname"].Value);
                            objAD.LastName = Convert.ToString(entry.Properties["lastname"].Value);
                            objAD.SearchText = SearchText;
                        }
                        listAD.Add(objAD);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listAD.DistinctBy(model => model.UserName).Where(model => model.UserName.ToUpper().Contains(SearchText.ToUpper())).ToList();
        }

        public static List<ActiveDirectoryModel> GetADDetailsFromIP(string strDomainName)
        {
            List<ActiveDirectoryModel> listAD = null;
            ActiveDirectoryModel objAD = null;
            ActiveDirectoryDetail objADDetails = null;
            DateTime? dtCurrentDate = null;
            DateTime? dtLastMonth = null;
            try
            {
                dtCurrentDate = DateTime.Now;
                dtLastMonth = DateTime.Now.AddDays(-30);
                objADDetails = new ActiveDirectoryDetail();
                using (AudissEntities objEntity = new AudissEntities())
                {
                    objADDetails = objEntity.ActiveDirectoryDetails.Where(model => model.DomainName.ToUpper() == strDomainName.ToUpper()).SingleOrDefault();
                    if (objADDetails != null)
                    {
                        listAD = new List<ActiveDirectoryModel>();
                        if (!string.IsNullOrEmpty(objADDetails.DomainName) && !string.IsNullOrEmpty(objADDetails.LDAPAddress))
                        {
                            DirectoryEntry entry = new DirectoryEntry(objADDetails.LDAPAddress, objADDetails.ADUserName, objADDetails.ADPassword);
                            object obj = entry.NativeObject;
                            DirectorySearcher search = new DirectorySearcher(entry);
                            search.Filter = ConstHelpers.Str_ADFilter;
                            search.PropertiesToLoad.Add(ConstHelpers.Str_ADKey_AcctName);
                            search.PropertiesToLoad.Add(ConstHelpers.Str_ADKey_Mail);
                            foreach (SearchResult resEnt in search.FindAll())
                            {
                                objAD = new ActiveDirectoryModel();
                                entry = resEnt.GetDirectoryEntry();
                                if (entry.Properties[ConstHelpers.Str_ADKey_AcctName].Value != null && entry.Properties[ConstHelpers.Str_ADKey_UsrAcctCtrl].Value != null)
                                {
                                    objAD.UserName = Convert.ToString(entry.Properties[ConstHelpers.Str_ADKey_AcctName].Value);
                                    objAD.EmailID = Convert.ToString(entry.Properties[ConstHelpers.Str_ADKey_Mail].Value);
                                    objAD.FirstName = Convert.ToString(entry.Properties["displayname"].Value);
                                    objAD.LastName = Convert.ToString(entry.Properties["lastname"].Value);
                                    objAD.CreatedDate = Convert.ToDateTime(entry.Properties["whenCreated"].Value);
                                    objAD.ADD_ID = objADDetails.Id;
                                }
                                listAD.Add(objAD);
                            }
                        }
                        listAD = listAD.Where(model => model.CreatedDate >= dtLastMonth).DistinctBy(model => model.UserName).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return listAD;
        }
    }
}
