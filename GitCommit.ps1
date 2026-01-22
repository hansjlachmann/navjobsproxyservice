git checkout master
git add .
git commit -m "Commit and Push from VS Code"
git push origin master


# regenerate service reference
dotnet-svcutil "http://localhost:7649/DynamicsNAVCarloTEST/WS/MOTORFORUM%20DRAMMEN/Codeunit/TestNavWs?wsdl" --outputFile ServiceReference/Reference.cs --namespace "*, ServiceReference"
