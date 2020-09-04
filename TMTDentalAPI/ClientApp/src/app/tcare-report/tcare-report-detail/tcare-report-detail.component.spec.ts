import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TCareReportDetailComponent } from './tcare-report-detail.component';

describe('TCareReportDetailComponent', () => {
  let component: TCareReportDetailComponent;
  let fixture: ComponentFixture<TCareReportDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TCareReportDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TCareReportDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
