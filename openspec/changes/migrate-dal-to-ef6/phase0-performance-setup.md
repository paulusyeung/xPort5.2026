# Phase 0 Performance Benchmarking - Setup Required

## Current Status
✅ Benchmark code created  
✅ Project structure ready  
❌ **Manual setup required** - Project references need to be added

## Files Created
1. `xPort5.EF6.PerformanceTests/PerformanceBenchmark.cs` - Benchmark logic
2. `xPort5.EF6.PerformanceTests/Program.cs` - Console app entry point
3. `xPort5.EF6.PerformanceTests/xPort5.EF6.PerformanceTests.csproj` - Project file
4. `xPort5.EF6.PerformanceTests/app.config` - Connection strings (already configured)

## Manual Steps Required

### Step 1: Add Compatibility Files to xPort5.EF6 Project
Open `xPort5.EF6/xPort5.EF6.csproj` in Visual Studio or a text editor and add these three lines to the `<ItemGroup>` section where other `<Compile Include=...` entries are:

```xml
<Compile Include="Article.Compatibility.cs" />
<Compile Include="Customer.Compatibility.cs" />
<Compile Include="T_Category.Compatibility.cs" />
```

### Step 2: Add References to xPort5.EF6 Project
In the same file, add these references to the `<ItemGroup>` section where other `<Reference Include=...` entries are:

```xml
<Reference Include="Gizmox.WebGUI.Common, Version=4.5.25701.0, Culture=neutral, PublicKeyToken=263fa4ef694acff6, processorArchitecture=MSIL" />
<Reference Include="Gizmox.WebGUI.Forms, Version=4.5.25701.0, Culture=neutral, PublicKeyToken=c508b41386c60f1d, processorArchitecture=MSIL" />
<Reference Include="System.Linq.Dynamic.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0f07ec44de6ac832, processorArchitecture=MSIL">
  <HintPath>..\packages\System.Linq.Dynamic.Core.1.3.7\lib\net452\System.Linq.Dynamic.Core.dll</HintPath>
</Reference>
```

### Step 3: Add Project Reference to xPort5.DAL
In `xPort5.EF6/xPort5.EF6.csproj`, add this to the `<ItemGroup>` section for project references:

```xml
<ProjectReference Include="..\xPort5.DAL\xPort5.DAL.csproj">
  <Project>{8E3643B9-85ED-45C0-8236-102EB497D18B}</Project>
  <Name>xPort5.DAL</Name>
</ProjectReference>
```

### Step 4: Build and Run
Once the above changes are made:

```powershell
cd c:\Projects\xPort5.2025\xPort5.EF6.PerformanceTests
dotnet build
dotnet run
```

## Alternative: Use Visual Studio
1. Open the solution in Visual Studio
2. Right-click on `xPort5.EF6` project → Add → Existing Item
3. Add the three `.Compatibility.cs` files
4. Right-click on `xPort5.EF6` → Add Reference
5. Add Gizmox references and xPort5.DAL project reference
6. Build and run the performance test project

## What the Benchmark Will Test
- **Load** operations (by ID)
- **LoadCollection** operations (all/filtered)
- **Save** operations (insert)

For T_Category, Customer, and Article entities.

## Expected Output
```
=== xPort5 Performance Benchmark: DAL vs EF6 ===
Warmup Iterations: 5
Test Iterations: 100
Target: EF6 within 10% of DAL performance

Benchmarking T_Category.Load()...
  DAL: 5.23ms | EF6: 5.67ms | Diff: +8.4% | PASS

... (more results)

=== BENCHMARK RESULTS ===
Operation            Entity          DAL (ms)     EF6 (ms)     Diff %     Status    
-------------------------------------------------------------------------------------
Load                 T_Category      5.23         5.67         +8.4       PASS      
LoadCollection       T_Category      23.45        24.12        +2.9       PASS      
...

Summary: 7 PASS | 0 FAIL | 2 SKIPPED
Overall: ✅ ALL TESTS PASSED
```

## Troubleshooting
If you encounter build errors, the issue is likely missing references. Double-check that all references from Steps 1-3 are added correctly.

---

**Note**: I attempted to automate these changes but the XML editing kept corrupting the project file. Manual editing is safer for .csproj files.
