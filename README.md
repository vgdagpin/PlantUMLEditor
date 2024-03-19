An editor for PlantUML https://plantuml.com/

Supported file extension: **.mdpuml**

Features
- Markdown render (using [Markdig](https://github.com/xoofx/markdig))
- PlantUML diagram render
- Render modes (Local and Remote) - Default to Remote
- You can build your own remote, clone https://github.com/vgdagpin/PlantUMLServer and deploy to your server

You can change the option in Tools > Options > Text Editor > PlantUML > Advance

Syntax:
```
Test 1234

**yeah**

@startuml Uml_1  
database    Db1
@enduml

The quick brown fox

	jumps

@startuml Uml_2
database    Db2
@enduml

   over 
---

@startmindmap
* Debian
** Ubuntu
*** Linux Mint
** LMDE
** Raspbian with a very long name
*** <s>Raspmbc</s> => OSMC
@endmindmap

--
the lazy
```
