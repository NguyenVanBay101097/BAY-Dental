import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerOverviewTreatmentHistoryComponent } from './partner-overview-treatment-history.component';

describe('PartnerOverviewTreatmentHistoryComponent', () => {
  let component: PartnerOverviewTreatmentHistoryComponent;
  let fixture: ComponentFixture<PartnerOverviewTreatmentHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerOverviewTreatmentHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerOverviewTreatmentHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
