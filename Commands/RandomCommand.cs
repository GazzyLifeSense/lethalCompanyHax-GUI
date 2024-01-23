using UnityEngine;
using GameNetcodeStuff;
using System;

namespace Hax;

public class RandomCommand : ICommand {
    static bool InverseTeleporterExists() {
        HaxObject.Instance?.ShipTeleporters.Renew();
        return Helper.InverseTeleporter is not null;
    }

    static ObjectPlacements<Transform, ShipTeleporter>? GetInverseTeleporterPlacements(Component target) {
        if (!InverseTeleporterExists()) return null;
        if (!Helper.InverseTeleporter.IsNotNull(out ShipTeleporter inverseTeleporter)) return null;

        Vector3 rotationOffset = new(-90.0f, 0.0f, 0.0f);

        ObjectPlacement<Transform, ShipTeleporter> teleporterPlacement = new(
            target.transform,
            inverseTeleporter,
            new Vector3(0.0f, 1.5f, 0.0f),
            rotationOffset
        );

        ObjectPlacement<Transform, ShipTeleporter> previousTeleporterPlacement = new(
            inverseTeleporter.transform.Copy().transform,
            inverseTeleporter,
            new Vector3(0.0f, 1.6f, 0.0f),
            rotationOffset
        );

        return new ObjectPlacements<Transform, ShipTeleporter>(
            teleporterPlacement,
            previousTeleporterPlacement
        );
    }

    static ObjectPlacements<Transform, PlaceableShipObject>? GetCupboardPlacements(Component target) {
        if (!Helper.GetUnlockable(Unlockable.CUPBOARD).IsNotNull(out PlaceableShipObject cupboard)) return null;

        ObjectPlacement<Transform, PlaceableShipObject> cupboardPlacement = new(
            target.transform,
            cupboard,
            new Vector3(0.0f, 1.75f, 0.0f),
            new Vector3(-90.0f, 0.0f, 0.0f)
        );

        ObjectPlacement<Transform, PlaceableShipObject> previousCupboardPlacement = new(
            cupboard.transform.Copy().transform,
            cupboard
        );

        return new ObjectPlacements<Transform, PlaceableShipObject>(
            cupboardPlacement,
            previousCupboardPlacement
        );
    }

    static Action TeleportPlayerToRandomLater(string playerId) => () => {
        if (!Helper.GetPlayer(playerId).IsNotNull(out PlayerControllerB targetPlayer)) {
            Console.Print("Player not found!");
            return;
        }

        ObjectPlacements<Transform, ShipTeleporter>? teleporterPlacements = GetInverseTeleporterPlacements(targetPlayer);

        if (teleporterPlacements is null) {
            Console.Print("Inverse Teleporter not found!");
            return;
        }

        Helper.CreateComponent<TransientBehaviour>()
              .Init((_) => Helper.PlaceObjectAtTransform(teleporterPlacements.Value.Placement), 6.0f)
              .Dispose(() => Helper.PlaceObjectAtTransform(teleporterPlacements.Value.PreviousPlacement));

        ObjectPlacements<Transform, PlaceableShipObject>? cupboardPlacements = GetCupboardPlacements(targetPlayer);

        if (cupboardPlacements is null) {
            Console.Print("Cupboard not found!");
            return;
        }

        Helper.CreateComponent<TransientBehaviour>()
              .Init((_) => Helper.PlaceObjectAtPosition(cupboardPlacements.Value.Placement), 6.0f)
              .Dispose(() => Helper.PlaceObjectAtTransform(cupboardPlacements.Value.PreviousPlacement));

        teleporterPlacements.Value.Placement.GameObject.PressTeleportButtonServerRpc();
    };

    static void TeleportPlayerToRandom(string playerId) {
        Helper.BuyUnlockable(Unlockable.INVERSE_TELEPORTER);
        Helper.CreateComponent<WaitForPredicate>()
              .SetPredicate(InverseTeleporterExists)
              .Init(TeleportPlayerToRandomLater(playerId));
    }

    public static void Execute(string playerId = "") {
        if (playerId.Length == 0) {
            Console.Print("Please Select a player to random tp");
            return;
        }

        TeleportPlayerToRandom(playerId);
    }
}
