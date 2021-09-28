using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using System.Threading.Tasks;

public class NetworkController : MonoBehaviour
{
    public GameObject networkClient;

    public static NetworkController networkControllerInstance;
    Dictionary<SteamId,NetworkUserInfo> networkUsers = new Dictionary<SteamId,NetworkUserInfo>();
    Lobby friendLobby;
    void Awake()
    {

        DontDestroyOnLoad(gameObject);
        if (networkControllerInstance == null) {
         networkControllerInstance = this;
        } else{
            Destroy(gameObject);
            return;
        }

        try
        {
            Steamworks.SteamClient.Init( 1208630 );
            string lobbyId = Steamworks.SteamApps.GetLaunchParam("JoinLobbyId");
            if (lobbyId == "") {
                Task<int> createLobbyTask = CreateFriendLobby();
            }
            else {
                ulong l = ulong.Parse(lobbyId);
                if (friendLobby.Id.IsValid) {
                    friendLobby.Leave();
                }
                friendLobby = new Lobby((SteamId)l);
                friendLobby.Join();
                
            }
            Steamworks.SteamApps.OnNewLaunchParameters += () => {
                lobbyId = Steamworks.SteamApps.GetLaunchParam("JoinLobbyId");
                if (lobbyId != "") {
                    ulong l = ulong.Parse(lobbyId);
                    friendLobby = new Lobby((SteamId)l);
                    friendLobby.Join();
                }
            };
            Steamworks.SteamNetworking.OnP2PSessionRequest = ( steamid ) =>
            {
                Debug.Log("P2P Session Request");
                // If we want to let this steamid talk to us
                Steamworks.SteamNetworking.AcceptP2PSessionWithUser( steamid );
                if (!networkUsers.ContainsKey(steamid))
                    GenerateUser(steamid);
            };

            System.Action<Lobby,Friend> MemberJoin = AddClient;
            System.Action<Lobby,Friend> MemberLeave = RemoveClient;
            System.Action<Lobby,Friend,Friend> MemberKick = RemoveClient;
            System.Action<Lobby,SteamId> ConnectToFriendLobbyAction = ConnectToFriendLobby;

            Steamworks.SteamMatchmaking.OnLobbyMemberJoined += MemberJoin;

            Steamworks.SteamMatchmaking.OnLobbyMemberDisconnected += MemberLeave;
            Steamworks.SteamMatchmaking.OnLobbyMemberLeave += MemberLeave;

            Steamworks.SteamMatchmaking.OnLobbyMemberBanned += MemberKick;
            Steamworks.SteamMatchmaking.OnLobbyMemberKicked += MemberKick;

            Steamworks.SteamFriends.OnGameLobbyJoinRequested += ConnectToFriendLobbyAction;

        }
        catch ( System.Exception e )
        {
            Debug.Log(e.ToString());
            // Something went wrong - it's one of these:
            //
            //     Steam is closed?
            //     Can't find steam_api dll?
            //     Don't have permission to play app?
            //
        }
    
    }

    void Update()
    {
	    Steamworks.SteamClient.RunCallbacks(); 
        if (friendLobby.Id.IsValid)
        while ( Steamworks.SteamNetworking.IsP2PPacketAvailable() )
        {
            var packet = Steamworks.SteamNetworking.ReadP2PPacket();
            if ( packet.HasValue )
            {
                SteamId id = packet.Value.SteamId;
                //GenerateUser(id);
                if (!networkUsers.ContainsKey(id))
                {
                    SteamFriends.RequestUserInformation(id);
                    GenerateUser(id);
                    /*Friend friend = new Friend(id);
                    UserInfo uinfo = new UserInfo(friend.Name,Instantiate(userObject,Vector3.zero,Quaternion.identity));
                    userObjectReverse.Add(uinfo.clientObj,uinfo);
                    userObjects.Add(id,uinfo);*/
                }

                MessageDecoder m = new MessageDecoder(packet.Value.Data);
                MessageType t = m.ReadMessageType();
                switch(t)
                {
                    case MessageType.PlayerAction:
                    {
                        networkUsers[id].controller.NetworkUpdate(m);
                    }   
                    break;
                    default:
                        Debug.Log("Invalid message type.");
                    break;
                }
            }
        }
    }

    public void AddClient(Lobby l, Friend f){
        if (!networkUsers.ContainsKey(f.Id))
                    GenerateUser(f.Id);
                    }
    public void ConnectToFriendLobby(Lobby lobby, SteamId f){}
    public void RemoveClient(Lobby lobby, Friend f){RemoveClient(lobby,f, new Friend());}
    public void RemoveClient(Lobby lobby, Friend f, Friend kicker){}
    private void GenerateUser(SteamId f) {
        networkUsers.Add(f,new NetworkUserInfo(f,Instantiate(networkClient,Vector3.zero,Quaternion.identity).GetComponent<NetworkUserController>()));

    }

    public void SendData(MessageEncoder bytes)
    {
        foreach(SteamId id in networkUsers.Keys)
        {
            var sent = SteamNetworking.SendP2PPacket( id, bytes.ToArray() );
            if (!sent)
                Debug.Log("Failed to send data.");
        }
    }

    public void SendData(MessageEncoder bytes, SteamId id)
    {
        var sent = SteamNetworking.SendP2PPacket( id, bytes.ToArray() );
        if (!sent)
            Debug.Log("Failed to send data.");
    }

    private async Task<int> CreateFriendLobby() {
        Lobby[] list = await SteamMatchmaking.LobbyList.RequestAsync();
        if (list != null && list.Length > 0) {
            var lobby = list[0];
            RoomEnter re = await lobby.Join();
            if (re == RoomEnter.Success)
            {
                Debug.Log("JoinLobby");
                friendLobby = lobby;
                return 1;
            }
        }
        if (!friendLobby.Id.IsValid) {
                Debug.Log("CreateLobby");
            friendLobby = (Lobby)(await SteamMatchmaking.CreateLobbyAsync());
            friendLobby.MaxMembers = 2;
            Steamworks.SteamFriends.SetRichPresence("connect","JoinLobbyId=" + friendLobby.Id);
            return 1;
        }
                Debug.Log("Failed to join any lobby");
        return 0;
    }
}
