import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerAdvanceListReportComponent } from './partner-advance-list-report.component';

describe('PartnerAdvanceListReportComponent', () => {
  let component: PartnerAdvanceListReportComponent;
  let fixture: ComponentFixture<PartnerAdvanceListReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerAdvanceListReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerAdvanceListReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
