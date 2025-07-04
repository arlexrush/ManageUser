﻿namespace ManageUser.Application.Features.Users.Queries.GetMyInfo
{
    public class GetMyInfoQueryResponse
    {
        public  string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }        
        public string Email { get; set; }        
        public List<string> Roles { get; set; }

    }
}
