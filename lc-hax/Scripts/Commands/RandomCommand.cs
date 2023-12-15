using UnityEngine;
using GameNetcodeStuff;
using System;

namespace Hax;

public class RandomCommand : ICommand {
    bool InverseTeleporterExists() {
        HaxObject.Instance?.ShipTeleporters.Renew();
        return Helper.InverseTeleporter.IsNotNull(out ShipTeleporter _);
    }

    ObjectPlacements<Transform, ShipTeleporter>? GetInverseTeleporterPlacements(Component target) {
        if (!this.InverseTeleporterExists()) return null;
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
            new Vector3(0.0f, 1.75f, 0.0f),
            rotationOffset
        );

        return new ObjectPlacements<Transform, ShipTeleporter>(
            teleporterPlacement,
            previousTeleporterPlacement
        );
    }

    ObjectPlacements<Transform, PlaceableShipObject>? GetCupboardPlacements(Component target) {
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

    Action TeleportPlayerToRandomLater(string[] args) => () => {
        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            Console.Print("Player not found!");
            return;
        }

        ObjectPlacements<Transform, ShipTeleporter>? teleporterPlacements = this.GetInverseTeleporterPlacements(targetPlayer);

        if (teleporterPlacements is null) {
            Console.Print("Inverse Teleporter not found!");
            return;
        }

        Helper.CreateComponent<TransientBehaviour>()
              .Init(Helper.PlaceObjectAtTransform(teleporterPlacements.Value.Placement), 6.0f)
              .Dispose(() => Helper.PlaceObjectAtTransform(teleporterPlacements.Value.PreviousPlacement).Invoke(0));

        ObjectPlacements<Transform, PlaceableShipObject>? cupboardPlacements = this.GetCupboardPlacements(targetPlayer);

        if (cupboardPlacements is null) {
            Console.Print("Cupboard not found!");
            return;
        }

        Helper.CreateComponent<TransientBehaviour>()
              .Init(Helper.PlaceObjectAtPosition(cupboardPlacements.Value.Placement), 6.0f)
              .Dispose(() => Helper.PlaceObjectAtTransform(cupboardPlacements.Value.PreviousPlacement).Invoke(0));

        teleporterPlacements.Value.Placement.GameObject.PressTeleportButtonServerRpc();
    };

    void TeleportPlayerToRandom(string[] args) {
        Helper.BuyUnlockable(Unlockable.CUPBOARD);
        Helper.BuyUnlockable(Unlockable.INVERSE_TELEPORTER);

        Helper.CreateComponent<WaitForPredicate>()
              .SetPredicate(this.InverseTeleporterExists)
              .Init(this.TeleportPlayerToRandomLater(args));
    }

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /random <player>");
            return;
        }

        this.TeleportPlayerToRandom(args);
    }
}
