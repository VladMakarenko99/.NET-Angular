export interface User {
    id: number;

    steamId: string;

    steamName: string;

    boughtServicesJson?: string;

    balance: number;
}