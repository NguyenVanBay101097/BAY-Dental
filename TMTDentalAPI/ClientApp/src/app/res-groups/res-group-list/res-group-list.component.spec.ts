import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResGroupListComponent } from './res-group-list.component';

describe('ResGroupListComponent', () => {
  let component: ResGroupListComponent;
  let fixture: ComponentFixture<ResGroupListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResGroupListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResGroupListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
