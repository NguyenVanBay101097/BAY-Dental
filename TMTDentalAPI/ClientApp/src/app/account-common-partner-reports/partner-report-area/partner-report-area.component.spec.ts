import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerReportAreaComponent } from './partner-report-area.component';

describe('PartnerReportAreaComponent', () => {
  let component: PartnerReportAreaComponent;
  let fixture: ComponentFixture<PartnerReportAreaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerReportAreaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerReportAreaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
