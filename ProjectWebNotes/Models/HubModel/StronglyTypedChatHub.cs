using Domain.IdentityModel;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProjectWebNotes.DbContextLayer;

namespace ProjectWebNotes.Models
{

    public interface IChatClient
    {
        //them nhieu phương thuc
        Task ReceiveMessage(string user, string message);
    }
    
    public class StronglyTypedChatHub : Hub
    {
        public async Task SendMessage(string user, string message, string room, bool join)
        {
            if (join)
            {
                await JoinRoom(room).ConfigureAwait(false);
                await Clients.Group(room).SendAsync("ReceiveMessage", user, " joined to " + room).ConfigureAwait(true);

            }
            else
            {
                await Clients.Group(room).SendAsync("ReceiveMessage", user, message).ConfigureAwait(true);

            }
        }

        public Task JoinRoom(string roomName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }


        public Task LeaveRoom(string roomName)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }


        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }


    }
}
