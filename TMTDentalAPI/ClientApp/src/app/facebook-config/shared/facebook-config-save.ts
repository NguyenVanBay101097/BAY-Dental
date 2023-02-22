import { FacebookConfigPageSave } from './facebook-config-page-save';

export class FacebookConfigSave {
    fbAccountName: string;
    fbAccountUserId: string;
    userAccessToken: string;
    configPages: FacebookConfigPageSave[];
}