import { Injectable } from '@angular/core';

@Injectable()
export class HostService {

    public host: string;

    constructor() {
        this.host = 'http://localhost:61832/';
    }

}