
## Compiling

**Use a recursive git clone.**

    git clone --recursive git://github.com/danielcrenna/TrueCraft.git

You need to restore Nuget packages. The easiest way is to open the solution up in monodevelop or visual studio or the like and build from there. You can alternatively acquire Nuget yourself and run this:

    mono path/to/nuget.exe restore

From the root directory of the git repository. Then run:

    xbuild

To compile it and you'll receive binaries in `TrueCraft.Launcher/bin/Debug/`. Run `[mono] TrueCraft.Launcher.exe` to run the client and connect to servers and play singleplayer and so on. Run `[mono] TrueCraft.Server.exe` to host a server for others to play on.

Note: if you have a problem with nuget connecting, run `mozroots --import --sync`.

Note: TrueCraft requires mono 4.0 or newer.

## Assets

TrueCraft is compatible with Minecraft beta 1.7.3 texture packs. We ship the Pixeludi Pack (by Wojtek Mroczek) by default. You can install the Mojang assets through the TrueCraft launcher if you wish.

## Disclaimer

NOT AN OFFICIAL MINECRAFT PRODUCT. NOT APPROVED BY OR ASSOCIATED WITH MOJANG.
