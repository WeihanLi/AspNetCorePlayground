using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace WeihanLi.AspNetCore.Authentication.QueryAuthentication
{
    public class QueryAuthenticationOptions : AuthenticationSchemeOptions
    {
        private string _userRolesQueryKey = "UserRoles";
        private string _userNameQueryKey = "UserName";
        private string _userIdQueryKey = "UserId";
        private string _delimiter = ",";

        public string UserIdQueryKey
        {
            get => _userIdQueryKey;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _userIdQueryKey = value;
                }
            }
        }

        public string UserNameQueryKey
        {
            get => _userNameQueryKey;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _userNameQueryKey = value;
                }
            }
        }

        public string UserRolesQueryKey
        {
            get => _userRolesQueryKey;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _userRolesQueryKey = value;
                }
            }
        }

        /// <summary>
        /// 自定义其他的 header
        /// key: QueryKey
        /// value: claimType
        /// </summary>
        public Dictionary<string, string> AdditionalQueryToClaims { get; } = new Dictionary<string, string>();

        public string Delimiter
        {
            get => _delimiter;
            set => _delimiter = string.IsNullOrEmpty(value) ? "," : value;
        }
    }
}
