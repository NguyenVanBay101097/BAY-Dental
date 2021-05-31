import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerAdvanceHistoryListComponent } from './partner-advance-history-list.component';

describe('PartnerAdvanceHistoryListComponent', () => {
  let component: PartnerAdvanceHistoryListComponent;
  let fixture: ComponentFixture<PartnerAdvanceHistoryListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerAdvanceHistoryListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerAdvanceHistoryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
