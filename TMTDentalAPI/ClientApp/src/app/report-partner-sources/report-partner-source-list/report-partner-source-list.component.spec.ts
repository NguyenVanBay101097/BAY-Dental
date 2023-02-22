import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportPartnerSourceListComponent } from './report-partner-source-list.component';

describe('ReportPartnerSourceListComponent', () => {
  let component: ReportPartnerSourceListComponent;
  let fixture: ComponentFixture<ReportPartnerSourceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReportPartnerSourceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportPartnerSourceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
