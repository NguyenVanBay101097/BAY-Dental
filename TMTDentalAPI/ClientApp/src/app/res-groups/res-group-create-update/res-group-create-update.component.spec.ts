import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResGroupCreateUpdateComponent } from './res-group-create-update.component';

describe('ResGroupCreateUpdateComponent', () => {
  let component: ResGroupCreateUpdateComponent;
  let fixture: ComponentFixture<ResGroupCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResGroupCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResGroupCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
