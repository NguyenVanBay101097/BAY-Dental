import { Inject, Injectable } from '@angular/core';
import {
    HttpTransportType,
    HubConnection,
    HubConnectionBuilder,
    HubConnectionState,
    LogLevel
} from '@microsoft/signalr';
import { BehaviorSubject, Subject } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Injectable({ providedIn: 'root' })
export class AppointmentSignalRService {
    appointmentChanged$ = new Subject();
    appointmentDeleted$ = new Subject();
    connectionEstablished$ = new BehaviorSubject<boolean>(false);

    private hubConnection: HubConnection;

    constructor(@Inject('BASE_API') private baseApi: string,
        private authService: AuthService) {
        this.createConnection();
        this.registerOnServerEvents();
        this.startConnection();
    }

    private createConnection() {
        this.hubConnection = new HubConnectionBuilder()
            .withUrl(this.baseApi + 'appointmentHub',
                {
                    skipNegotiation: true,
                    transport: HttpTransportType.WebSockets,
                    accessTokenFactory: () => this.authService.getAuthorizationToken()
                })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();
    }

    private startConnection() {
        if (this.hubConnection.state === HubConnectionState.Connected) {
            return;
        }

        this.hubConnection.start().then(
            () => {
                console.log('Hub connection started!');
                this.connectionEstablished$.next(true);
            },
            error => console.error(error)
        );
    }

    private registerOnServerEvents(): void {
        this.hubConnection.on('created', (data: any) => {
            this.appointmentChanged$.next(data);
        });

        this.hubConnection.on('updated', (data: any) => {
            this.appointmentChanged$.next(data);
        });

        this.hubConnection.on('deleted', (data: any) => {
            this.appointmentDeleted$.next(data);
        });
    }
}