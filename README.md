# Architect
**Version: 0.0.3**  
**Unity 2021.3.14f1 or higher**  
**(You also can try old versions)**
# Documentation
## Introduction
**Architect** is a lite tool to view UML like scheme of unity monolite project. **Architect** is just an analog of architecture analyzer in Microsoft Visual Studio. This tool is raw and has a lot of details. For using the product you need to have base knowledges of UML schemes and project architecture.
## Code format
For correct work with the tool you must format scripts in a project according to the following rules:
### 1. Module
Use *[ParentModule("ModuleName")]* class attribute to refer a class to the module.  
Example:  
>[ParentModule("Weapon")]  
>public class Weapon
### 2. Direct references
Use direct references in your scripts  
Do this:
>private Weapon gun;  

Not this:
>private GameObject gunObject;
>
>gunObject.GetComponent<Weapon>();  
  
*For now the system can't detect not direct references.  
Please don't use singletones. Use instances.*
  
## How to use
For open an UML view window click *Architect->View* on the top bar.  
**Red blocks** - modules  
**Blue blocks** - classes  
**White arrows** - direct references  
