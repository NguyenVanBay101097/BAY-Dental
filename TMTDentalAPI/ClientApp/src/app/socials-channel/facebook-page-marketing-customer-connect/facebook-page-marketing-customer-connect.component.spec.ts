import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookPageMarketingCustomerConnectComponent } from './facebook-page-marketing-customer-connect.component';

describe('FacebookPageMarketingCustomerConnectComponent', () => {
  let component: FacebookPageMarketingCustomerConnectComponent;
  let fixture: ComponentFixture<FacebookPageMarketingCustomerConnectComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookPageMarketingCustomerConnectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookPageMarketingCustomerConnectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
