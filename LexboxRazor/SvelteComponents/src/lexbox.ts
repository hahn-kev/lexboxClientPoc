import type { DotNet } from "@microsoft/dotnet-js-interop";

declare global {
  interface Window {
    lexbox: {
      DotNetServiceProvider: typeof DotNetServiceProvider;
    }
  }
}

export enum DotNetServices {
  LexboxApi = 'LexboxApi',
}

var SERVICE_KEYS = Object.values(DotNetServices);

export class DotNetServiceProvider {
  static services: Record<string, DotNet.DotNetObject> = {};

  static setService(key: string, service: DotNet.DotNetObject) {
    console.log('set-service');
    this.validateServiceKey(key);
    this.services[key] = service;
  }

  static getService(key: string): DotNet.DotNetObject {
    this.validateServiceKey(key);
    return this.services[key];
  }

  private static validateServiceKey(key: string) {
    if (!SERVICE_KEYS.includes(key as DotNetServices)) {
      throw new Error(`Invalid service key: ${key}. Valid vales are: ${SERVICE_KEYS.join(', ')}`);
    }
  }
}

window.lexbox = {
  DotNetServiceProvider
};
