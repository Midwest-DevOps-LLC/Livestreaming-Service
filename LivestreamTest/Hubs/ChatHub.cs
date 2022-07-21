using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LivestreamTest.Hubs
{
    public class ChatHub : Hub
    {
        private MDO.RESTDataEntities.Standard.UserSession _userSession;
        public MDO.RESTDataEntities.Standard.UserSession UserSession(string auth)
        {
            if (_userSession == null)
            {

                if (string.IsNullOrEmpty(auth) == false)
                {
                    MDO.RESTServiceRequestor.Standard.SessionRequest sessionRequest = new MDO.RESTServiceRequestor.Standard.SessionRequest("https://api.midwestdevops.com/", auth, "8729f38f-2068-4f13-a612-d39d102aee23");
                    var response = sessionRequest.Verify();

                    _userSession = response.Data;
                }
            }

            return _userSession;
        }

        public async Task SendMessage(string auth, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(auth))
                    return;

                if (string.IsNullOrEmpty(message))
                    return;

                var userSession = UserSession(auth);

                if (userSession == null)
                    return;

                foreach (var emote in EmoteHandler.Emotes.data)
                {
                    if (emote.name == ":/")
                        continue;

                    message = message.Replace(emote.name, $"<img src='{emote.images.url_1x}' />", StringComparison.OrdinalIgnoreCase);
                }

                await Clients.All.SendAsync("ReceiveMessage", userSession.Username, message);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task SendScreen(string base64Image)
        {
            try
            {
                //byte[] bytes = Convert.FromBase64String(base64Image);

                //Image image;
                //using (MemoryStream ms = new MemoryStream(bytes))
                //{
                //    image = Image.FromStream(ms);

                //    image.Save("testfromthing.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //}

                

                //b.Save("testfromthing.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                await Clients.Others.SendAsync("ReceiveScreen", base64Image);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
