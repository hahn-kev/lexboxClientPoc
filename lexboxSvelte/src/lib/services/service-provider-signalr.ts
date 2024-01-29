import type { HubConnection } from "@microsoft/signalr";
import { getHubProxyFactory } from "../generated-signalr-client/TypedSignalR.Client";
import type { LexboxApi } from "./lexbox-api";
import {LexboxServiceProvider, LexboxServices} from "./service-provider";

export function SetupSignalR(connection: HubConnection) {
    const hubFactory = getHubProxyFactory("ILexboxApiHub");
    const hubProxy = hubFactory.createHubProxy(connection);
    LexboxServiceProvider.setService(LexboxServices.LexboxApi, hubProxy satisfies LexboxApi);
}