import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerDebtListComponent } from './partner-customer-debt-list.component';

describe('PartnerCustomerDebtListComponent', () => {
  let component: PartnerCustomerDebtListComponent;
  let fixture: ComponentFixture<PartnerCustomerDebtListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerDebtListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerDebtListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
