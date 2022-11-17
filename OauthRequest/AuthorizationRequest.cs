﻿namespace PocAspNetCoreBlazorAntDesign.OauthRequest
{
    public class AuthorizationRequest
    {
        public AuthorizationRequest() { }
        /// <summary>
        /// Response Type, is required
        /// </summary>
        public string response_type { get; set; }

        /// <summary>
        /// Client Id, is required
        /// </summary>

        public string client_id { get; set; }

        /// <summary>
        /// Redirect Uri, is optional
        /// The redirection endpoint URI MUST be an absolute URI as defined by
        /// [RFC3986] Section 4.3
        /// </summary>

        public string redirect_uri { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public string scope { get; set; }

        /// <summary>
        /// Return the state in the result 
        /// if it was present in the client authorization request
        /// </summary>
        public string state { get; set; }
    }
}
