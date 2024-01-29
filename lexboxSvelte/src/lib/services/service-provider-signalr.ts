import { getHubProxyFactory } from "../generated-signalr-client/TypedSignalR.Client";
import type { LexboxApi } from "./lexbox-api";
import {LexboxServiceProvider, LexboxServices} from "./service-provider";

export function Setup() {
    const hubFactory = getHubProxyFactory("ILexboxApiHub");
    const hubProxy = hubFactory.createHubProxy(null);
    LexboxServiceProvider.setService(LexboxServices.LexboxApi, hubProxy satisfies LexboxApi);
}