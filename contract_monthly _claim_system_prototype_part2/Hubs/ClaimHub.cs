using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace contract_monthly__claim_system_prototype_part2.Hubs
{
    public class ClaimHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            //example, if the user is in admin role, add to Admins graoup.For demo we use query param
            var role= Context.GetHttpContext()?.Request.Query["role"].ToString();
            var userid = Context.User?.Identity?.Name ?? Context.ConnectionId;
            if (role == "admin") Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            //join personal group for status updates
            Groups.AddToGroupAsync(Context.ConnectionId, userid);
            return base.OnConnectedAsync();

        }
    }
}
