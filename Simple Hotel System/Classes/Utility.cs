//using cinema_ticketing.BusinessLogic.Setting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Text;

namespace cinema_ticketing.Classes
{
    public abstract class Utility
    {
        public const string DataTableJsonNoDataFormat = "{\"draw\": 0, \"recordsTotal\": 0, \"recordsFiltered\": 0, \"data\":[]}";
        public const string JsonNoDataFormat = "{\"IsSuccess\": 0, \"Message\": 0, \"data\":[]}";

        //public static string GetCompileDateTimeCache(Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
        //{
        //    return cache.GetOrCreate<string>(Constant.CACHE_LAST_UPDATED,
        //       cacheEntry =>
        //       {
        //           //cacheEntry.SetSlidingExpiration(TimeSpan.FromSeconds(5));              //多久没request 而expires?
        //           cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);      //无论有没有Request，直接expires?
        //           return File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString("yyyy-MM-dd hh:mm:ss tt");
        //       });
        //}

        //public static void UpdateOperatorAccessTime(Microsoft.Extensions.Caching.Memory.IMemoryCache cache, int operatorID)
        //{
        //    _ = cache.GetOrCreate<string>(Constant.CACHE_OPERATOR_ACCESS + operatorID.ToString(),
        //    cacheEntry =>
        //    {
        //        cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
        //        BLOperator.UpdateAccessTime(operatorID);
        //        return "";
        //    });
        //}

        //public static void UpdateOperatorAccessTime(int operatorID)
        //{
        //    BLOperator.UpdateAccessTime(operatorID);
        //}

        public static string GetCompileDateTime()
        {
            //get this application compile date time
            //return File.GetCreationTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString("yyyy-MM-dd hh:mm:ss tt");
            return File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString("yyyy-MM-dd hh:mm:ss tt");
        }

        public static string GetIPAddress(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            //httpContext = Request.HttpContext
            string ipAddress = String.Empty;
            if (httpContext.Request.Headers.ContainsKey("X-Real-IP"))
            {
                ipAddress = httpContext.Request.Headers["X-Real-IP"];
            }
            else if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = httpContext.Request.Headers["X-Forwarded-For"];
            }
            else
            {
                ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
            }
            return ipAddress;
        }

        public static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true);
            return builder.Build();
        }

        public static string ShowLink(Microsoft.AspNetCore.Http.HttpContext context, string actionName, string controllerName, string url, string querystring = "")
        {
            string currentPath = context.Request.Path.ToString();
            string currentQuery = context.Request.QueryString.ToString();

            string[] arr = currentPath.Split("/");
            string currentActionName = arr[arr.Length - 1];
            string currentControllerName = "";
            if (arr.Length >= 2)
            {
                currentControllerName = arr[arr.Length - 2];
            }
            if (currentActionName == actionName && string.IsNullOrEmpty(currentControllerName) == false && currentControllerName == controllerName && querystring == currentQuery)
            {
                return "#";
            }
            else
            {
                return url;
                //return url.Action(controllerName, actionName).ToString();
                //@Url.Action("Dashboard","Main")

                //var requestContext = HttpContext.Current.Request.RequestContext;
                //new UrlHelper(requestContext).Action("Index", "MainPage");
                //var requestContext = HttpContext.Current.Request.RequestContext;
                //new Microsoft.AspNetCore.Mvc.Routing.UrlHelper(requestContext).Action("Index", "MainPage");
            }
            //return fileName;
        }

        public static Boolean GetControllerNAction(Microsoft.AspNetCore.Http.HttpContext context, ref string controller, ref string action)
        {
            string currentPath = context.Request.Path.ToString();
            string[] arr = currentPath.Split("/");
            action = arr[arr.Length - 1];
            //string currentControllerName = "";
            if (arr.Length >= 2)
            {
                controller = arr[arr.Length - 2];
                return true;
            }
            return false;
        }

        public static string ShowBootstrapMsg(string msg, bool isDanger = false, bool closeButton = true)
        {
            string type;
            if (isDanger)
            {
                type = "alert-danger";
            }
            else
            {
                type = "alert-primary";
            }
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            //str.Append("<div class=\"alert alert-danger rounded\" role=\"alert\">");
            str.Append("<div class=\"alert alert-dismissible " + type + " rounded\" role=\"alert\">");
            str.Append(msg);
            if (closeButton)
            {
                str.Append("<button type='button' class='close' data-dismiss='alert' aria-label='Close'>");
                str.Append("<span aria-hidden='true'>&times;</span>");
                str.Append("</button>");
            }
            str.Append("</div>");
            return str.ToString();
        }

        public static string NoNull(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return obj.ToString();
        }

        public static Microsoft.AspNetCore.Http.IFormFile NoNullIFormFile(object obj)
        {
            if (obj == null) return null;
            return (Microsoft.AspNetCore.Http.IFormFile)obj;
        }

        public static string FormatSQLDate(object obj, bool noTime = false)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            if (noTime)
            {
                return Convert.ToDateTime(obj).ToString("yyyy-MM-dd");
            }
            else
            {
                return Convert.ToDateTime(obj).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public static string FormatSQLDateTime(Object obj)
        {
            if (obj == DBNull.Value || obj == null)
            {
                return string.Empty;
            }
            if (obj.Equals(""))
            {
                return string.Empty;
            }
            return Convert.ToDateTime(obj).ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string FormatSQLDate24H(object obj, bool noTime = false)
        {
            //for data table sorting, need to be 24 hours format
            if (obj == null)
            {
                return string.Empty;
            }
            if (noTime)
            {
                return Convert.ToDateTime(obj).ToString("yyyy-MM-dd");
            }
            else
            {
                return Convert.ToDateTime(obj).ToString("yyyy-MM-dd HH:mm:ss");
            }

            //return Convert.ToDateTime(obj).ToString("yyyy-MM-dd hh:mm:ss tt", new CultureInfo("en-US"));
        }

        public static string FormatSQLTime(object obj, bool noTime = false)
        {
            //for data table sorting, need to be 24 hours format
            if (obj == null)
            {
                return string.Empty;
            }
            return Convert.ToDateTime(obj).ToString("HH:mm:ss");

            //return Convert.ToDateTime(obj).ToString("yyyy-MM-dd hh:mm:ss tt", new CultureInfo("en-US"));
        }

        public static decimal ConvNum(object obj)
        {
            if (obj == null) return 0;
            string str = obj.ToString();

            decimal myNum = 0;
            if (Decimal.TryParse(str, out myNum))
            {
                return myNum;
            }
            else
            {
                return 0;
            }
        }

        public static decimal NoNullDec(object obj)
        {
            return ConvNum(obj);
        }

        public static float NoNullFloat(object obj)
        {
            if (obj == null) return 0;
            string str = obj.ToString();

            float myNum = 0;
            if (float.TryParse(str, out myNum))
            {
                return myNum;
            }
            else
            {
                return 0;
            }
        }

        public static (bool isOk, DateTime dateTime) NoNullDateTime(Object obj)
        {
            if (obj == DBNull.Value || obj == null)
            {
                return (false, Convert.ToDateTime("1990-01-01"));
            }
            return (true, Convert.ToDateTime(obj));
        }

        public static DateTime NoNullDateTime2(Object obj)
        {
            if (obj == DBNull.Value || obj == null)
            {
                return Convert.ToDateTime("1990-01-01");
            }
            return Convert.ToDateTime(obj);
        }

        public static bool NoNullBool(object obj)
        {
            if (obj == DBNull.Value || obj == null) return false;

            return Convert.ToBoolean(obj);
        }

        public static int NoNullInt(object obj)
        {
            if (obj == null) return 0;
            //return Convert.ToInt32(obj);
            string str = obj.ToString();

            int myNum = 0;
            if (int.TryParse(str, out myNum))
            {
                return myNum;
            }
            else
            {
                return 0;
            }
        }

        public static int BoolTo01(bool value)
        {
            if (value)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static string BoolToYN(bool value)
        {
            if (value)
            {
                return "Y";
            }
            else
            {
                return "N";
            }
        }

        public static string FormatAmt(decimal value)
        {
            return value.ToString("###,###,##0.00");
        }

        public static string GetBankNameById(int id)
        {
            switch (id)
            {
                case 3:
                    return "Maybank";

                case 6:
                    return "Hong Leong Bank";

                default:
                    return "";
            }
        }

        public static string GetIpAddress(HttpContext httpContext)
        {
            //httpContext = Request.HttpContext
            string ipAddress = String.Empty;
            if (httpContext.Request.Headers.ContainsKey("X-Real-IP"))
            {
                ipAddress = httpContext.Request.Headers["X-Real-IP"];
            }
            else if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = httpContext.Request.Headers["X-Forwarded-For"];
            }
            else
            {
                ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
            }
            return ipAddress;
        }

        public static string GetBrowserName(string agent)
        {
            if (agent.IndexOf("edge") > -1)
            {
                return "MS Edge";
            }
            else if (agent.IndexOf("edg/") > -1)
            {
                return "Edge (chromium based)";
            }
            else if (agent.IndexOf("opr") > -1)
            {
                return "Opera";
            }
            else if (agent.IndexOf("chrome") > -1)
            {
                return "Chrome";
            }
            else if (agent.IndexOf("trident") > -1)
            {
                return "MS IE";
            }
            else if (agent.IndexOf("firefox") > -1)
            {
                return "Mozilla Firefox";
            }
            else if (agent.IndexOf("safari") > -1)
            {
                return "Safari";
            }
            else
            {
                return "other";
            }
        }

        public static bool IsCustomerloggedIn(HttpContext context)
        {
            int customerId = Utility.NoNullInt(context.Request.Cookies["customer_id"]);

            if (customerId != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsAdminLoggedIn(HttpContext context)
        {
            int customerId = Utility.NoNullInt(context.Request.Cookies["admin_id"]);

            if (customerId != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            string hashedInput = HashPassword(plainPassword);
            return hashedInput == hashedPassword;
        }

        public static string GetUrl(IUrlHelper url, string? action, string? controller, object? values, string? protocol)
        {
            return url.Action(action, controller, values, protocol) ?? "";
        }

        //    public static string GetCurrentPageName_xx(Microsoft.AspNetCore.Http.HttpContext context)
        //{
        //    //var aa = _httpContextAccessor.HttpContext.Request.Path.ToString;

        //    //var url = context.HttpContext.Request.Path.ToString();
        //    //string a = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetFullUrl(Request);

        //    //HttpRequest request = HttpContext.Current.Request;
        //    //string url = request.Url.ToString();
        //    //return url;

        //    //string sPath = Microsoft.AspNetCore.Http.HttpContext.Current.Request.Url.AbsolutePath;
        //    return context.Request.Path.ToString();

        //    //System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
        //    //string sRet = oInfo.Name;
        //    //return sRet;
        //}

        //public static IConfigurationBuilder getConfiguration()
        //{
        //    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true);
        //    return builder.Build();
        //}

        //public static string showLanguageFlag()
        //{
        //    System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        //    switch(currentCulture.Name) {
        //        case "en-US":
        //            return "English";
        //        case "zh-CN":
        //            return "Chinese";
        //        default:
        //            return "Dunno" + currentCulture.Name;
        //    }
        //}


    }

    
}