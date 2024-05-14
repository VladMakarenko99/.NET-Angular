export interface Service {
    id: string;

    name: string;

    description: string;

    optionsToSelect: string[];

    selectedOption: string;

    expireDate: string;
}