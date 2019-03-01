using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace WeihanLi.AspNetCore.Authentication.HeaderAuthentication
{
    public class HeaderAuthenticationOptions : AuthenticationSchemeOptions
    {
        private string _userRolesHeaderName = "UserRoles";
        private string _userNameHeaderName = "UserName";
        private string _userIdHeaderName = "UserId";
        private string _delimiter = ",";

        public string UserIdHeaderName
        {
            get => _userIdHeaderName;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _userIdHeaderName = value;
                }
            }
        }

        public string UserNameHeaderName
        {
            get => _userNameHeaderName;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _userNameHeaderName = value;
                }
            }
        }

        public string UserRolesHeaderName
        {
            get => _userRolesHeaderName;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _userRolesHeaderName = value;
                }
            }
        }

        /// <summary>
        /// 自定义其他的 header
        /// key: headerName
        /// value: claimType
        /// </summary>
        public Dictionary<string, string> AdditionalHeaderToClaims { get; } = new Dictionary<string, string>();

        public string Delimiter
        {
            get => _delimiter;
            set => _delimiter = string.IsNullOrEmpty(value) ? "," : value;
        }
    }
}
