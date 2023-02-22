import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleReportItemDetailComponent } from './sale-report-item-detail.component';

describe('SaleReportItemDetailComponent', () => {
  let component: SaleReportItemDetailComponent;
  let fixture: ComponentFixture<SaleReportItemDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleReportItemDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleReportItemDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
