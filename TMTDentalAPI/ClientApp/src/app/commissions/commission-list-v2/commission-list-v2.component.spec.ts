import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionListV2Component } from './commission-list-v2.component';

describe('CommissionListV2Component', () => {
  let component: CommissionListV2Component;
  let fixture: ComponentFixture<CommissionListV2Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionListV2Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionListV2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
