param($installPath, $toolsPath, $package, $project)
// search for <Content Include="Translations.xml">
// and add if non existant
// <CopyToOutputDirectory>Always</CopyToOutputDirectory>
// in order to have this:
// <ItemGroup>
//    <Content Include="Translations.xml">
//      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
//    </Content>
//  </ItemGroup>
// in csproj file
