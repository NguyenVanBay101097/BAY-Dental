import { FacebookConfigPageDisplay } from './facebook-config-page-display';

export class FacebookConfigDisplay {
    id: string;
    fbAccountName: string;
    fbAccountUserId: string;
    userAccessToken: string;
    configPages: FacebookConfigPageDisplay[];
}