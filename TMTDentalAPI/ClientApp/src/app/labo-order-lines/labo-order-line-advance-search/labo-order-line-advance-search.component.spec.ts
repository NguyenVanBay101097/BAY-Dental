import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderLineAdvanceSearchComponent } from './labo-order-line-advance-search.component';

describe('LaboOrderLineAdvanceSearchComponent', () => {
  let component: LaboOrderLineAdvanceSearchComponent;
  let fixture: ComponentFixture<LaboOrderLineAdvanceSearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderLineAdvanceSearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderLineAdvanceSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
