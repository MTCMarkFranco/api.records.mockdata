
# https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/security/how-to-grant-data-plane-role-based-access?tabs=built-in-definition%2Ccsharp&pivots=azure-interface-bicep#permission-model
az cosmosdb sql role definition list --resource-group "rg-ConversationCopilot" --account-name "cdb-conversational"

az cosmosdb sql role assignment create --account-name cdb-conversational --resource-group rg-ConversationCopilot --principal-id 77af0205-9c85-413a-ab32-93dcce629a73 --role-definition-id "00000000-0000-0000-0000-000000000002" --scope "/"

az cosmosdb sql role assignment create --account-name cdb-conversational --resource-group rg-ConversationCopilot --principal-id 310310b5-d51d-4708-ae7a-082f9489b44f --role-definition-id "00000000-0000-0000-0000-000000000002" --scope "/subscriptions/28d10200-70b0-476c-b004-c6ae29265897/resourceGroups/rg-ConversationCopilot/providers/Microsoft.DocumentDB/databaseAccounts/cdb-conversational"

