import { Service } from "./service.model";

export interface Purchase {
    service?: Service;

    steamId: string;
    
}