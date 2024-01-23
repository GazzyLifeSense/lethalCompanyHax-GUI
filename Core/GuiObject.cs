#pragma warning disable CS8618

using System.Collections;
using System.Data.SqlTypes;
using UnityEngine;


namespace Hax;

public class GuiObject : MonoBehaviour
{

    private int windowId = 114514;
    private int playerWindowId = 114515;
    private bool isMenuOpen;
    private bool cursorIsLocked = true;
    private float lastToggleTime = 0f;
    private const float toggleCooldown = 0.5f;
    private int frames = 0;
    private Vector2 scrollPos = Vector2.zero;

    private string timescale = "1.0";
    private string stunDuration = "5";
    private string shovelHitForce = "1";
    private string[] playerNames;
    private int selPlayerId = 0;
    private string secondPlayerId = "";
    private bool isInfiniteSprintEnabled = true;
    private string moneyInput = "200";

    void OnGUI()
    {
        GUI.Label(new Rect(10f, 5f, 200f, 30f), "LCHax By Lotus V0.1");
        if (isMenuOpen)
        {
            Rect windowRect = new Rect(20, 20, 300, 500); // Adjust size and position as needed
            windowRect = GUILayout.Window(windowId, windowRect, DrawMenuWindow, "Lethal Company1");

            Rect playerWindowRect = new Rect(330, 20, 300, 500); // Adjust size and position as needed
            windowRect = GUILayout.Window(playerWindowId, playerWindowRect, DrawPlayerFuncWindow, "Lethal Company2");
        }
    }

    void OnEnable()
    {
        InputListener.onInsertKeyPress += this.Toggle;

    }

    void OnDisable()
    {
        InputListener.onInsertKeyPress -= this.Toggle;
    }

    // Controll the window open;
    void Toggle()
    {
        if (Time.time - lastToggleTime > toggleCooldown)
        {
            isMenuOpen = !isMenuOpen;
            lastToggleTime = Time.time;
        }
        if (StartOfRound.Instance != null)
        {
            if (isMenuOpen)
            {
                // Menu opened, unlock cursor
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                cursorIsLocked = false;
            }
            else if (!cursorIsLocked)
            {
                // To prevent not being able to use ESC menu. We only free up the cursor once
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                cursorIsLocked = true;
            }
        }
    }

    void FixedUpdate()
    {
        // per 60 frames update player list
        frames++;
        if (frames % 60 == 0)
        {
            playerNames = PlayersCommand.Execute();
        }
    }
    void DrawMenuWindow(int id)
    {
        GUILayout.Label($"Infinite sprint: {(isInfiniteSprintEnabled ? "enabled" : "disabled")}");
        if (GUILayout.Button("Heal"))
        {
            HealCommand.Execute();
        }
        // timescale
        timescale = GUILayout.TextField(timescale, 5);
        if (GUILayout.Button("Change timescale"))
        {
            TimescaleCommand.Execute(timescale);
        }

        // stun
        GUILayout.Label("Stun duration:");
        stunDuration = GUILayout.TextField(stunDuration, 5);
        if (GUILayout.Button("Stun Enemy"))
        {
            StunCommand.Execute(stunDuration);
        }
        GUILayout.Label($"Stun on LeftClick: {(Settings.EnableStunOnLeftClick ? "enabled" : "disabled")}");
        if (GUILayout.Button("Stun on LeftClick"))
        {
            Settings.EnableStunOnLeftClick = !Settings.EnableStunOnLeftClick;
        }

        // explode
        if (GUILayout.Button("Explode all landmine"))
        {
            ExplodeCommand.Execute("landmine");
        }
        if (GUILayout.Button("Explode all jetpack"))
        {
            ExplodeCommand.Execute("jetpack");
        }

        // Set shovel hit force
        GUILayout.Label($"Shovel hit force: {Settings.ShovelHitForce}");
        shovelHitForce = GUILayout.TextField(shovelHitForce, 10);
        if (GUILayout.Button("Modify ShovelHitForce"))
        {
            ShovelCommand.Execute(shovelHitForce);
        }

        if (GUILayout.Button("Log Current Location"))
        {
            LocationCommand.Execute();
        }

        // Add money
        moneyInput = GUILayout.TextField(moneyInput, 10);
        if (GUILayout.Button("Add/Subtract Money"))
        {
            MoneyCommand.Execute(moneyInput);
        }

        // lock/unlock all doors
        if (GUILayout.Button("Lock All doors"))
        {
            LockCommand.Execute();
        }
        if (GUILayout.Button("Unlock All doors"))
        {
            UnlockCommand.Execute();
        }

        GUI.DragWindow();
    }

    void DrawPlayerFuncWindow(int id)
    {
        // player list
        GUILayout.Label("Select a Player to trigger functions");
        GUILayout.Label("d: dead, s: surviving");
        scrollPos = GUILayout.BeginScrollView(
           scrollPos, GUILayout.Height(150));
        selPlayerId = GUILayout.SelectionGrid(selPlayerId, playerNames, 1);
        GUILayout.EndScrollView();
        if (GUILayout.Button("Unselect any player"))
        {
            selPlayerId = -1;
        }

        // kill
        if (GUILayout.Button("Kill(unselect for suicide)"))
        {
            KillCommand.Execute(selPlayerId == -1 ? "" : selPlayerId.ToString());
        }
        if (GUILayout.Button("Kill All"))
        {
            KillCommand.KillAllPlayers();
        }

        // hate
        if (GUILayout.Button("Pumpkin around player"))
        {
            new PumpkinCommand().Execute(selPlayerId == -1 ? "" : selPlayerId.ToString());
        }
        if (GUILayout.Button("ChibakuTensei"))
        {
            new ChibakuTenseiCommand().Execute(selPlayerId == -1 ? "" : selPlayerId.ToString());
        }
        if (GUILayout.Button("hate"))
        {
            HateCommand.Execute(selPlayerId == -1 ? "" : selPlayerId.ToString());
        }

        // teleport
        if (GUILayout.Button("Home Tp(unselect for all players)"))
        {
            HomeCommand.Execute(selPlayerId == -1 ? "" : selPlayerId.ToString());
        }
        if (GUILayout.Button("Random Tp"))
        {
            RandomCommand.Execute(selPlayerId == -1 ? "" : selPlayerId.ToString());
        }
        if (GUILayout.Button("Tp to player"))
        {
            TeleportCommand.Execute(selPlayerId == -1 ? [""] : [selPlayerId.ToString()]);
        }

        GUILayout.Label($"Second playerId: {secondPlayerId}");
        secondPlayerId = GUILayout.TextField(secondPlayerId, 3);
        if (GUILayout.Button("Tp player to secondPlayer"))
        {
            TeleportCommand.Execute(selPlayerId == -1 ? [""] : [selPlayerId.ToString(), secondPlayerId]);
        }

        if (GUILayout.Button("Unmount Hax"))
        {
            Loader.Unload();
        }

        GUI.DragWindow();
    }
}
