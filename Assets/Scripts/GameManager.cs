using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    //burası spawn kısmı 
    [SerializeField] List<Transform> spawns = new List<Transform>();
    [SerializeField] List<Transform> spawnsWalk = new List<Transform>();
    [SerializeField] List<Transform> spawnsTurret = new List<Transform>();
    int randSpawn;


    //bu kısım player kontrol kısmı
    //yazı elemanına bir referans verelim
    [SerializeField] public TMP_Text playersText;
    //oyuncuları depolayacak liste ya da array
    GameObject[] players;
    //aktif oyuncuların isimlerini depolayacak bir liste
    List<string> activePlayers = new List<string>();

    int checkPlayers = 0;
    int previousPlayerCount;

    void Start()
    {        
        randSpawn = Random.Range(0, spawns.Count);
        PhotonNetwork.Instantiate("Player", spawns[randSpawn].position, spawns[randSpawn].rotation);
        Invoke("SpawnEnemy", 5f);
        previousPlayerCount = PhotonNetwork.PlayerList.Length;
    }

    private void Update()
    {
        if(PhotonNetwork.PlayerList.Length < previousPlayerCount)
        {
            ChangePlayerList();
        }
        previousPlayerCount = PhotonNetwork.PlayerList.Length;
    }


    public void SpawnEnemy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < spawnsWalk.Count; i++)
            {
                PhotonNetwork.Instantiate("WalkEnemy", spawnsWalk[i].position, spawnsWalk[i].rotation);
            }
            for (int i = 0; i < spawnsTurret.Count; i++)
            {
                PhotonNetwork.Instantiate("Turret", spawnsTurret[i].position, spawnsTurret[i].rotation);
            }
        }
    }

    public void ChangePlayerList()
    {
        photonView.RPC("PlayerList", RpcTarget.All);
    }

    [PunRPC]
    public void PlayerList()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        activePlayers.Clear();

        //aktif oyuncular listesini oluştur
        //bi tane listenin içerisindeki bütün elemanlar için şunları şunları yap
        foreach(GameObject player in players)
        {
            //eğer oyuncu hayattaysa
            if(player.GetComponent<PlayerController>().dead == false)
            {
                //aktif oyuncular içerisine ismini ekle
                activePlayers.Add(player.GetComponent<PhotonView>().Owner.NickName);
            }
        }

        //eğer oyunda 1 kişi kaldıysa yani kazanan kişi belli olduysa düşmanları durdur
        if(activePlayers.Count <= 1 && checkPlayers > 0)
        {
            PlayerPrefs.SetString("winner", activePlayers[0]);
            //oyundaki bütün düşmanları bul 
            var enemies = GameObject.FindGameObjectsWithTag("enemy");
            //var variable demek variable da değişken demek
            //listedeki düşmanlrın hepsini öldür
            foreach(GameObject enemy in enemies)
            {
                enemy.GetComponent<Enemy>().ChangeHealth(999999);
            }
            Invoke("EndGame", 5f);
        }

        checkPlayers++;


        //bişey olana kadar bişeyler yap
        //for(int i = 0; i > 10; i++)
    }

    void EndGame()
    {
        //lobiye geri dön
        PhotonNetwork.LoadLevel("Lobby");
    }

    public void ExitGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        //odadan ayrılan kişiyi menüye yönlendir
        SceneManager.LoadScene(0);
        //oyuncu listesini güncelle
        ChangePlayerList();
    }
}
