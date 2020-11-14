import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerOverviewSaleOrderLineComponent } from './partner-overview-sale-order-line.component';

describe('PartnerOverviewSaleOrderLineComponent', () => {
  let component: PartnerOverviewSaleOrderLineComponent;
  let fixture: ComponentFixture<PartnerOverviewSaleOrderLineComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerOverviewSaleOrderLineComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerOverviewSaleOrderLineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
