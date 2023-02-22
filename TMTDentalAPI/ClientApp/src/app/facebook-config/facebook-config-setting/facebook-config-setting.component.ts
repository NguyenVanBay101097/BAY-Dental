import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FacebookConfigService } from '../shared/facebook-config.service';
import { FacebookConfigDisplay } from '../shared/facebook-config-display';
import { FacebookConfigPageService } from '../shared/facebook-config-page.service';

@Component({
  selector: 'app-facebook-config-setting',
  templateUrl: './facebook-config-setting.component.html',
  styleUrls: ['./facebook-config-setting.component.css']
})
export class FacebookConfigSettingComponent implements OnInit {
  config: FacebookConfigDisplay;
  configId: string;
  constructor(private router: Router, private facebookConfigService: FacebookConfigService,
    private route: ActivatedRoute, private configPageService: FacebookConfigPageService) { }

  ngOnInit() {
    this.configId = this.route.snapshot.paramMap.get('id');
    this.loadConfig();
  }

  loadConfig() {
    this.facebookConfigService.get(this.configId).subscribe(result => {
      this.config = result;
    });
  }

  logout() {
    localStorage.removeItem('facebook_config_id');
    this.router.navigate(['/facebook-config/connect']);
  }

  syncData(page: any) {
    this.configPageService.syncData(page.id).subscribe(() => {
      console.log('sync success');
    });
  }
}
