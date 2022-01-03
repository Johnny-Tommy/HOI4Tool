# HOI4Tool
## Little modding tool for the game "Hearts of Iron IV"
(Written in C# and XAML (WPF). Windows OS and dotnetcore 3.1 are required to run this application.)

Special thanks to the following projects:
- encoding libary (a.o. DDS): https://github.com/Nominom/BCnEncoder.NET
- file parser "format of Paradox":  https://github.com/nickbabcock/Pdoxcl2Sharp

### Features
#### DDS-Viewer
![image](https://user-images.githubusercontent.com/94912164/143608228-f0da8c1f-5119-4c1f-b06f-c7e15ea381b3.png)

#### Editor for insignia (armies, armygroups and fleets)
![image](https://user-images.githubusercontent.com/94912164/147831839-9bbbdfc4-6536-47f8-9598-6bca2724ff55.png)

### How to use / first steps
To use the insignia editor, you have to go to the setup first to configure the directory paths. This step should be self explanatory. After this you have to create a backup of the files which can be edited with this tool. To do this, just press the backup button. (The recovery doesn't work yet - just copy the files from the backup folder to their original location if you want to recover your files)

### Future steps

- Recovery function
- Import function for new graphics to use for an insignia
- Optimize XAML
- Optimize the open points (see preprocessor instructions)
- Translate the application into english
- Batch operations to edit multiple icons at the same time
