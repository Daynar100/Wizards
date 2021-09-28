
using Steamworks;
using Steamworks.Data;
public class NetworkUserInfo
{
    public SteamId id;
    public NetworkUserController controller;    
    public NetworkUserInfo(SteamId id, NetworkUserController controller) {
        this.id = id;
        this.controller = controller;
    }
}