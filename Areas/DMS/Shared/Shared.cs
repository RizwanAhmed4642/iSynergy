using iSynergy.Areas.DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iSynergy.DataContexts;
using iSynergy.Areas.HR.Models;
namespace iSynergy.Areas.DMS.Shared
{
    public static class ArchivingFunctions 
    {
        public static IEnumerable<DocumentFolder> getAuthorizedFolders(Employee thisGuy)
        {
            using (var db = new ArchivingDb())
            {
                var roleIds = (from role in db.UserDocumentAccessRoles
                               where role.EmployeeId == thisGuy.EmployeeId
                               select role.DocumentAccessRoleId).ToList();

                var folders = (from role in db.FolderRights
                                     where roleIds.Contains(role.DocumentAccessRoleId)
                                     select role.DocumentFolder).ToList();

                return folders;
            }
        }

        public static IEnumerable<DocumentCategory> getAuthorizedCategories(DocumentFolder folder, Employee thisGuy)
        {
            using (var db = new ArchivingDb())
            {
                var roleIds = (from role in db.UserDocumentAccessRoles
                               where role.EmployeeId == thisGuy.EmployeeId
                               select role.DocumentAccessRoleId).ToList();

                var categories = (from role in db.CategoryRights
                                  where roleIds.Contains(role.DocumentAccessRoleId)
                                  select role.DocumentCategory).ToList();

                return categories
                        .Where(x => x.DocuementFolderId == folder.DocumentFolderId)
                        .ToList();
            }
        }

        public static IEnumerable<DocumentCategory> getAllCategories(DocumentFolder folder)
        {
            using (var db = new ArchivingDb())
            {

                return db.DocumentCategories
                        .Where(x => x.DocuementFolderId == folder.DocumentFolderId)
                        .ToList();
            }
        }
    }
}