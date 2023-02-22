import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TCareReportListComponent } from './tcare-report-list.component';

describe('TCareReportListComponent', () => {
  let component: TCareReportListComponent;
  let fixture: ComponentFixture<TCareReportListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TCareReportListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TCareReportListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
