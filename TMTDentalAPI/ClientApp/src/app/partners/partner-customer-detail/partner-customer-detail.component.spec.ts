import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerDetailComponent } from './partner-customer-detail.component';

describe('PartnerCustomerDetailComponent', () => {
  let component: PartnerCustomerDetailComponent;
  let fixture: ComponentFixture<PartnerCustomerDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
