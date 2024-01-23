using System;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class TeleportCommand : ICommand {
    static Vector3? GetCoordinates(string[] args) {
        bool isValidX = float.TryParse(args[0], out float x);
        bool isValidY = float.TryParse(args[1], out float y);
        bool isValidZ = float.TryParse(args[2], out float z);

        return !isValidX || !isValidY || !isValidZ ? null : new Vector3(x, y, z);
    }

    static Result TeleportToPlayer(string[] args) {
        PlayerControllerB? targetPlayer = Helper.GetPlayer(args[0]);
        PlayerControllerB? currentPlayer = Helper.LocalPlayer;

        if (targetPlayer is null || currentPlayer is null) {
            return new Result(message: "Player not found!");
        }

        currentPlayer.TeleportPlayer(targetPlayer.transform.position);
        return new Result(true);
    }

    static Result TeleportToPosition(string[] args) {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB currentPlayer)) {
            return new Result(message: "Player not found!");
        }

        Vector3? coordinates = GetCoordinates(args);

        if (coordinates is null) {
            return new Result(message: "Invalid coordinates!");
        }

        currentPlayer.TeleportPlayer(coordinates.Value);
        return new Result(true);
    }

    static Action TeleportPlayerToPositionLater(PlayerControllerB player, Vector3 position) => () => {
        HaxObject.Instance?.ShipTeleporters.Renew();

        if (!Helper.Teleporter.IsNotNull(out ShipTeleporter teleporter)) {
            Console.Print("ShipTeleporter not found!");
            return;
        }

        GameObject newTransform = player.transform.Copy();
        newTransform.transform.position = position;

        Helper.SwitchRadarTarget(player);
        teleporter.PressTeleportButtonServerRpc();

        Vector3 rotationOffset = new(-90.0f, 0.0f, 0.0f);
        Vector3 positionOffset = new(0.0f, 1.6f, 0.0f);

        ObjectPlacement<Transform, ShipTeleporter> teleporterPlacement = new(
            newTransform.transform,
            teleporter,
            positionOffset,
            rotationOffset
        );

        ObjectPlacement<Transform, ShipTeleporter> previousTeleporterPlacement = new(
            teleporter.transform.Copy().transform,
            teleporter,
            positionOffset,
            rotationOffset
        );

        Helper.CreateComponent<TransientBehaviour>()
              .Init((_) => Helper.PlaceObjectAtPosition(teleporterPlacement), 6.0f)
              .Dispose(() => Helper.PlaceObjectAtPosition(previousTeleporterPlacement));
    };

    static Result TeleportPlayerToPosition(PlayerControllerB player, Vector3 position) {
        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        Helper.CreateComponent<WaitForPredicate>()
              .SetPredicate(Helper.TeleporterExists)
              .Init(TeleportPlayerToPositionLater(player, position));

        return new Result(true);
    }

    static Result TeleportPlayerToPosition(string[] args) {
        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB player)) {
            return new Result(message: "Player not found!");
        }

        Vector3? coordinates = GetCoordinates(args[1..]);

        return coordinates is null
            ? new Result(message: "Invalid coordinates!")
            : TeleportPlayerToPosition(player, coordinates.Value);
    }

    static Result TeleportPlayerToPlayer(string[] args) {
        PlayerControllerB? sourcePlayer = Helper.GetPlayer(args[0]);
        PlayerControllerB? targetPlayer = Helper.GetPlayer(args[1]);

        return sourcePlayer is null || targetPlayer is null
            ? new Result(message: "Player not found!")
            : TeleportPlayerToPosition(sourcePlayer, targetPlayer.transform.position);
    }

    public static void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /tp <player>");
            Console.Print("Usage: /tp <x> <y> <z>");
            Console.Print("Usage: /tp <player> <x> <y> <z>");
            Console.Print("Usage: /tp <player> <player>");
            return;
        }

        Result result = new(message: "Invalid arguments!");

        if (args.Length is 1) {
            result = TeleportToPlayer(args);
        }

        else if (args.Length is 2) {
            result = TeleportPlayerToPlayer(args);
        }

        else if (args.Length is 3) {
            result = TeleportToPosition(args);
        }

        else if (args.Length is 4) {
            result = TeleportPlayerToPosition(args);
        }

        if (!result.Success) {
            Console.Print(result.Message);
        }
    }
}
