using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tuexamapi.Models;
using tuexamapi.Util;

namespace tuexamapi.DAL
{
    public static class TuExamContextExtensions
    {
        public static void EnsureSeedData(this TuExamContext context)
        {
            SeedMasterData(context);
            UpdateDatabaseDescriptions(context);
        }

        public static void SeedMasterData(TuExamContext context)
        {
            if (context.Users != null && !context.Users.Any())
            {
                var user = new User { UserName = "admin", Password = DataEncryptor.Encrypt("admin123"), Create_On = DateUtil.Now(), Create_By = "system", Update_On = DateUtil.Now(), Update_By = "system" };
                var staff = new Staff() { FirstName = "admin", LastName = "super", Create_On = DateUtil.Now(), Create_By = "system", Update_On = DateUtil.Now(), Update_By = "system" };
                staff.User = user;
                context.Staffs.Add(staff);
                context.SaveChanges();
            }
            if (context.Facultys != null && !context.Facultys.Any())
            {
                var List = new List<Faculty>
                {
                    new Faculty { FacultyName ="คณะเภสัชศาสตร์"  },
                    new Faculty { FacultyName ="คณะเศรษฐศาสตร์"  },
                    new Faculty { FacultyName ="คณะแพทยศาสตร์"  },
                    new Faculty { FacultyName ="คณะทันตแพทยศาสตร์"  },
                    new Faculty { FacultyName ="คณะนิติศาสตร์"  },
                    new Faculty { FacultyName ="คณะพยาบาลศาสตร์"  },
                    new Faculty { FacultyName ="คณะพาณิชยศาสตร์และการบัญชี"  },
                    new Faculty { FacultyName ="คณะรัฐศาสตร์"  },
                    new Faculty { FacultyName ="คณะวารสารศาสตร์และสื่อสารมวลชน"  },
                    new Faculty { FacultyName ="คณะวิทยาการเรียนรู้และศึกษาศาสตร์"  },
                    new Faculty { FacultyName ="คณะวิทยาศาสตร์และเทคโนโลยี"  },
                    new Faculty { FacultyName ="คณะวิศวกรรมศาสตร์"  },
                    new Faculty { FacultyName ="คณะศิลปกรรมศาสตร์"  },
                    new Faculty { FacultyName ="คณะศิลปศาสตร์"  },
                    new Faculty { FacultyName ="คณะสถาปัตยกรรมศาสตร์และการผังเมือง"  },
                    new Faculty { FacultyName ="คณะสหเวชศาสตร์"  },
                    new Faculty { FacultyName ="คณะสังคมวิทยาและมานุษยวิทยา"  },
                    new Faculty { FacultyName ="คณะสังคมสงเคราะห์ศาสตร์"  },
                    new Faculty { FacultyName ="คณะสาธารณสุขศาสตร์"  },
                    new Faculty { FacultyName ="วิทยาลัยแพทยศาสตร์นานาชาติจุฬาภรณ์"  },
                    new Faculty { FacultyName ="วิทยาลัยนวัตกรรม"  },
                    new Faculty { FacultyName ="วิทยาลัยนานาชาติ ปรีดี พนมยงค์"  },
                    new Faculty { FacultyName ="วิทยาลัยพัฒนศาสตร์ ป๋วย อึ๊งภากรณ์"  },
                    new Faculty { FacultyName ="สถาบันเทคโนโลยีนานาชาติสิรินธร"  }
                };
                List = List.OrderBy(o => o.FacultyName).ToList();
                List.ForEach(s => context.Facultys.Add(s));
                context.SaveChanges();
            }
        }

        public static void UpdateDatabaseDescriptions(TuExamContext context)
        {
            var contextType = typeof(TuExamContext);
            var props = contextType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var prop in props)
            {
                if (prop.PropertyType.GetGenericArguments().Length == 0)
                    continue;

                var tableType = prop.PropertyType.GetGenericArguments()[0];

                string fullTableName = prop.Name;
                Regex regex = new Regex(@"(\[\w+\]\.)?\[(?<table>.*)\]");
                Match match = regex.Match(fullTableName);
                string tableName;
                if (match.Success)
                    tableName = match.Groups["table"].Value;
                else
                    tableName = fullTableName;

                foreach (var prop2 in tableType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    var attrs = prop2.GetCustomAttributes(typeof(DisplayAttribute), false);
                    if (attrs.Length > 0)
                    {
                        var columnName = prop2.Name;
                        var description = ((DisplayAttribute)attrs[0]).Name;
                        string strGetDesc = "select [value] from fn_listextendedproperty('MS_Description','schema','dbo','table',N'" + tableName + "','column',null) where objname = N'" + columnName + "';";
                        using (var command = context.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = strGetDesc;
                            command.CommandType = CommandType.Text;

                            context.Database.OpenConnection();

                            object val = null;
                            using (var result = command.ExecuteReader())
                            {
                                while (result.Read())
                                {
                                    val = result;
                                }
                            }
                            if (val == null)
                            {
                                StringBuilder sql = new StringBuilder();
                                sql.Append(@"EXEC sp_addextendedproperty ");
                                sql.Append(" @name = N'MS_Description', @value = N'" + description + "',");
                                sql.Append(" @level0type = N'Schema', @level0name = 'dbo',");
                                sql.Append(" @level1type = N'Table',  @level1name = [" + tableName + "],");
                                sql.Append(" @level2type = N'Column', @level2name = [" + columnName + "];");

                                int result = context.Database.ExecuteSqlCommand(sql.ToString());
                            }
                            else
                            {
                                StringBuilder sql = new StringBuilder();
                                sql.Append(@"EXEC sp_updateextendedproperty ");
                                sql.Append(" @name = N'MS_Description', @value = N'" + description + "',");
                                sql.Append(" @level0type = N'Schema', @level0name = 'dbo',");
                                sql.Append(" @level1type = N'Table',  @level1name = [" + tableName + "],");
                                sql.Append(" @level2type = N'Column', @level2name = [" + columnName + "];");

                                int result = context.Database.ExecuteSqlCommand(sql.ToString());
                            }
                            context.Database.CloseConnection();
                        }

                    }
                }
            }
        }


    }
}
