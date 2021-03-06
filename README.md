# HOI4Tool
## Little modding tool for the game "Hearts of Iron IV"
(Written in C# and XAML (WPF). Windows OS and dotnetcore 3.1 is required to run this application.)

Special thanks goes to the folowing projects:
- encoding libary (a.o. DDS): https://github.com/Nominom/BCnEncoder.NET
- file parser "format of Paradox":  https://github.com/nickbabcock/Pdoxcl2Sharp

### Features
#### DDS-Viewer
![image](https://user-images.githubusercontent.com/94912164/143608228-f0da8c1f-5119-4c1f-b06f-c7e15ea381b3.png)

#### Editor for insignia (armies, armygroups and fleets)
![image](https://user-images.githubusercontent.com/94912164/147831839-9bbbdfc4-6536-47f8-9598-6bca2724ff55.png)

#### Create custom icons from images
![image](https://user-images.githubusercontent.com/94912164/169599934-d071fce7-6e65-41a8-ba08-f3c8d87dedd1.png)

### How to use / first steps
To use the insignia editor, you have to go to setup first. Here you've to configure the directory paths - this should me self explanatory. After this you have to create an Backup of the files which could be edited with this tool. Just press the backup button. (the recovery doesn't work yet - just copy the backuped files to the origin if you want to recover your files)

### Future steps

- Recovery function
- optimize XAML
- optimize the open points (see preprocessor instructions)
- translate the application into english
- batch operations to edit multiple icons at the same time
