import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboOrderLineListComponent } from './labo-order-line-list.component';

describe('LaboOrderLineListComponent', () => {
  let component: LaboOrderLineListComponent;
  let fixture: ComponentFixture<LaboOrderLineListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboOrderLineListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboOrderLineListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
